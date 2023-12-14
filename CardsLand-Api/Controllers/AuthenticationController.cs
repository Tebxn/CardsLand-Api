using CardsLand_Api.Entities;
using CardsLand_Api.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using CardsLand_Api.Implementations;
using Org.BouncyCastle.Cms;

namespace CardsLand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IDbConnectionProvider _connectionProvider;
        private readonly ITools _tools;
        private readonly IBCryptHelper _bCryptHelper;


        public AuthenticationController(IDbConnectionProvider connectionProvider, ITools tools, IBCryptHelper bCryptHelper)
        {
            _connectionProvider = connectionProvider;
            _tools = tools;
            _bCryptHelper = bCryptHelper;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login(UserEnt entity)
        {
            ApiResponse<UserEnt> response = new ApiResponse<UserEnt>();

            try
            {
                bool passwordIsValid = false;

                if (string.IsNullOrEmpty(entity.User_Email) || string.IsNullOrEmpty(entity.User_Password))
                {
                    response.ErrorMessage = "Email and password are required";
                    response.Code = 400;
                    return BadRequest(response);
                }

                using (var connection = _connectionProvider.GetConnection())
                {
                    var data = await connection.QueryFirstOrDefaultAsync<UserEnt>("Login",
                        new { entity.User_Email },
                        commandType: CommandType.StoredProcedure);

                    if (data != null) 
                    {
                        passwordIsValid = _tools.CheckPassword(entity.User_Password, data.User_Password);

                        if (!passwordIsValid)
                        {
                            response.ErrorMessage = "Incorrect email or password";
                            response.Code = 404;
                            return NotFound(response);
                        }
                        response.Success = true;
                        response.Code = 200;
                        response.Data = data;
                        response.Data.User_Password = "";
                        response.Data.UserToken = _tools.GenerateToken(data.User_Id.ToString(), data.User_IsAdmin.ToString());
                        return Ok(response);
                    }
                    response.ErrorMessage = "Incorrect email or password";
                    response.Code = 404;
                    return NotFound(response);
                }
            }
            catch (SqlException ex)
            {
                await _tools.AddError(ex.Message);
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("RegisterAccount")]
        public async Task<IActionResult> RegisterAccount(UserEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                if (string.IsNullOrEmpty(entity.User_Nickname) || string.IsNullOrEmpty(entity.User_Password) || string.IsNullOrEmpty(entity.User_Email))
                {
                    response.ErrorMessage = "Nickname, email and password are required.";
                    response.Code = 400;
                    return BadRequest(response);
                }

                var User_Activation_Code = _tools.GenerateRandomCode(8);
                entity.User_Password = _tools.Encrypt(entity.User_Password);

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryFirstOrDefaultAsync("RegisterAccount",
                    new { entity.User_Nickname, entity.User_Email, entity.User_Password, User_Activation_Code },
                    commandType: CommandType.StoredProcedure);

                    if (data != null)
                    {
                        entity.User_Id = data.User_Id;
                        string body = _tools.MakeHtmlNewUser(entity, User_Activation_Code);
                        string recipient = entity.User_Email;

                        bool emailIsSend = _tools.SendEmail(recipient, "CardsLand", body);
                        if (emailIsSend)
                        {
                            response.Success = true;
                            response.Code = 200;
                            return Ok(response);
                        }
                        else
                        {
                            response.ErrorMessage = "Email already exists";
                            response.Code = 500;
                            return BadRequest(response);
                        }

                    }
                    else
                    {
                        response.ErrorMessage = "Email already exists";
                        response.Code = 500;
                        return BadRequest(response);
                    }
                }
            }
            catch (SqlException ex)
            {
                await _tools.AddError(ex.Message);
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                response.Code = 500;
                return BadRequest(response);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("PwdRecovery")]
        public async Task<IActionResult> PwdRecovery(UserEnt entity)
        {
            ApiResponse<UserEnt> response = new ApiResponse<UserEnt>();

            try
            {
                if (string.IsNullOrEmpty(entity.User_Email))
                {
                    response.ErrorMessage = "Email is required";
                    response.Code = 400;
                    return BadRequest(response);
                }

                using (var connection = _connectionProvider.GetConnection())
                {
                    var data = await connection.QueryFirstOrDefaultAsync<UserEnt>("PwdRecovery",
                        new { entity.User_Email },
                        commandType: CommandType.StoredProcedure);


                    if (data != null)
                    {
                        var randomPassword = _tools.GenerateRandomCode(8);
                        var hashedPassword = _tools.Encrypt(randomPassword);
                        entity.User_Password = hashedPassword;

                        string body = _tools.MakeHtmlPassRecovery(data, randomPassword);
                        string recipient = entity.User_Email;

                        var updatePass = await connection.ExecuteAsync("UpdateTempPassword",
                        new { data.User_Id, hashedPassword },
                        commandType: CommandType.StoredProcedure);


                        if (updatePass != 0)
                        {
                            bool emailIsSend = _tools.SendEmail(recipient, "CardLand - Restaurar Contraseña", body);

                            if (emailIsSend)
                            {
                                response.Success = true;
                                response.Code = 200;
                                return Ok(response);
                            }
                            else
                            {
                                response.ErrorMessage = "Error enviando el correo";
                                response.Code = 500;
                                return BadRequest(response);
                            }

                        }
                        else
                        {
                            response.ErrorMessage = "Error actualizando contraseña";
                            response.Code = 500;
                            return BadRequest(response);
                        }

                    }
                    else
                    {
                        response.ErrorMessage = "Correo no valido o usuario inactivo";
                        response.Code = 500;
                        return BadRequest(response);
                    }
                }
            }
            catch (SqlException ex)
            {
                await _tools.AddError(ex.Message);
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("UpdateNewPassword")]
        public async Task<IActionResult> UpdateNewPassword(UserEnt entity)
        {
            ApiResponse<UserEnt> response = new ApiResponse<UserEnt>();

            try
            {
                using (var connection = _connectionProvider.GetConnection())
                {
                    entity.User_Id = long.Parse(_tools.Decrypt(entity.SecuredId));

                    var getPass = await connection.QueryFirstOrDefaultAsync<UserEnt>("GetEncryptedPass",
                         new { entity.User_Id },
                         commandType: CommandType.StoredProcedure);

                    if (getPass != null)
                    {
                        getPass.User_Password = _tools.Decrypt(getPass.User_Password);
                        if (getPass.User_Password != entity.User_TempPassword)
                        {
                            response.ErrorMessage = "La contraseña temporal proporcionada no es valida";
                            response.Code = 500;
                            return BadRequest(response);
                        }
                        else
                        {
                            entity.User_Password = _tools.Encrypt(entity.User_Password);

                            var data = await connection.ExecuteAsync("UpdateNewPassword",
                                new { entity.User_Id, entity.User_Password },
                                commandType: CommandType.StoredProcedure);

                            if (data != 0)
                            {
                                response.Success = true;
                                response.Code = 200;
                                return Ok(response);
                            }
                            else
                            {
                                response.ErrorMessage = "Error al actualizar su contraseña nueva";
                                response.Code = 500;
                                return BadRequest(response);
                            }
                        }
                    }
                    else
                    {
                        response.ErrorMessage = "Error al actualizar su contraseña nueva";
                        response.Code = 500;
                        return BadRequest(response);
                    }

                }
            }
            catch (SqlException ex)
            {
                await _tools.AddError(ex.Message);
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("ActivateAccount")]
        public async Task<IActionResult> ActivateAccount(UserEnt entity)
        {
            ApiResponse<UserEnt> response = new ApiResponse<UserEnt>();

            try
            {
                using (var connection = _connectionProvider.GetConnection())
                {
                    entity.User_Id = long.Parse(_tools.Decrypt(entity.SecuredId));

                    var getActivationCode = await connection.QueryFirstOrDefaultAsync<UserEnt>("GetActivationCode",
                         new { entity.User_Id },
                         commandType: CommandType.StoredProcedure);

                    if (getActivationCode != null)
                    {
                        if (getActivationCode.User_Activation_Code != entity.User_Activation_Code)
                        {
                            response.ErrorMessage = "El codigo de activacion proporcionado no es valido";
                            response.Code = 500;
                            return BadRequest(response);
                        }
                        else
                        {

                            var data = await connection.ExecuteAsync("ActivateAccount",
                                new { entity.User_Id },
                                commandType: CommandType.StoredProcedure);

                            if (data != 0)
                            {
                                response.Success = true;
                                response.Code = 200;
                                return Ok(response);
                            }
                            else
                            {
                                response.ErrorMessage = "Error al activar su cuenta";
                                response.Code = 500;
                                return BadRequest(response);
                            }
                        }
                    }
                    else
                    {
                        response.ErrorMessage = "Error al activar su cuenta";
                        response.Code = 500;
                        return BadRequest(response);
                    }

                }
            }
            catch (SqlException ex)
            {
                await _tools.AddError(ex.Message);
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPut]
        [Route("DisableAccount")]
        public async Task<IActionResult> DisableAccount(UserEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                if (entity.User_Id == 0)
                {
                    response.ErrorMessage = "User_Id can't be empty.";
                    response.Code = 400;
                    return BadRequest(response);
                }

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryFirstOrDefaultAsync<UserEnt>("DisableAccount",
                        new { entity.User_Id },
                        commandType: CommandType.StoredProcedure);

                    response.Success = true;
                    response.Code = 200;
                    return Ok(response);
                }
            }
            catch (SqlException ex)
            {
                await _tools.AddError(ex.Message);
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                response.Code = 500;
                return BadRequest(response);
            }
        }
    }
}
