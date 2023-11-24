using CardsLand_Api.Entities;
using CardsLand_Api.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using CardsLand_Api.Implementations;

namespace CardsLand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IDbConnectionProvider _connectionProvider;
        private readonly ITools _tools;

        public AuthenticationController(IDbConnectionProvider connectionProvider, ITools tools)
        {
            _connectionProvider = connectionProvider;
            _tools = tools;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login(UserEnt entity)
        {
            ApiResponse<UserEnt> response = new ApiResponse<UserEnt>();

            try
            {
                if (string.IsNullOrEmpty(entity.User_Email) || string.IsNullOrEmpty(entity.User_Password))
                {
                    response.ErrorMessage = "Email and password are required";
                    response.Code = 400;
                    return BadRequest(response);
                }

                using (var connection = _connectionProvider.GetConnection())
                {
                    var data = await connection.QueryFirstOrDefaultAsync<UserEnt>("Login",
                        new { entity.User_Email},
                        commandType: CommandType.StoredProcedure);

                    bool passwordIsValid = _tools.CheckPassword(entity.User_Password, data.User_Password);
                    if (data == null || !passwordIsValid)
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
            }
            catch (SqlException ex)
            {
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
                if (string.IsNullOrEmpty(entity.User_Nickname) || string.IsNullOrEmpty(entity.User_Password)|| string.IsNullOrEmpty(entity.User_Email))
                {
                    response.ErrorMessage = "Nickname, email and password are required.";
                    response.Code = 400;
                    return BadRequest(response);
                }

                var User_Activation_Code = _tools.GenerateRandomCode(8);
                entity.User_Password = _tools.Encrypt(entity.User_Password);

                using (var context = _connectionProvider.GetConnection())
                {
                    string body = _tools.MakeHtmlNewUser(entity.User_Nickname, User_Activation_Code);
                    string recipient = entity.User_Email;

                    bool emailIsSend = _tools.SendEmail(recipient, "CardsLand", body);
                    if (emailIsSend)
                    {
                        var data = await context.ExecuteAsync("RegisterAccount",
                        new { entity.User_Nickname, entity.User_Email, entity.User_Password, User_Activation_Code },
                        commandType: CommandType.StoredProcedure);

                        if (data != 0)
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
                        response.ErrorMessage = "Error Sending activation email";
                        response.Code = 500;
                        return BadRequest(response);
                    }
                }
            }
            catch (SqlException ex)
            {
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                response.Code = 500;
                return BadRequest(response);
            }
        }

        //[HttpPost]
        //[Route("RecoverAccount")]
        //public async Task<IActionResult> RecoverAccount(UserEnt entity)
        //{
        //    ApiResponse<string> response = new ApiResponse<string>();

        //    try
        //    {
        //        if (string.IsNullOrEmpty(entity.User_Email))
        //        {
        //            response.ErrorMessage = "Email is required.";
        //            response.Code = 400;
        //            return BadRequest(response);
        //        }

        //        /string temporalPassword = _tools.CreatePassword(8);

        //        using (var context = _connectionProvider.GetConnection())
        //        {
        //            var data = await context.QueryFirstOrDefaultAsync<UserEnt>("RecoverAccount",
        //                new { entity.User_Email, TemporalPassword = temporalPassword },
        //                commandType: CommandType.StoredProcedure);

        //            if (data != null)
        //            {
        //                string body = "Your new password to access PokeLand is: " + temporalPassword +
        //                    "\nPlease log in with your new password and change it.";
        //                string recipient = entity.User_Email;
        //                _tools.SendEmail(recipient, "PokeLand Recover Account", body);

        //                response.Success = true;
        //                response.Code = 200;
        //                return Ok(response);
        //            }
        //            else
        //            {
        //                response.ErrorMessage = "Error Sending email";
        //                response.Code = 500;
        //                return BadRequest(response);
        //            }
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        response.ErrorMessage = "Unexpected Error: " + ex.Message;
        //        response.Code = 500;
        //        return BadRequest(response);
        //    }
        //}

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
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                response.Code = 500;
                return BadRequest(response);
            }
        }

        [HttpPut]
        [Route("ActivateAccount")]
        public async Task<IActionResult> ActivateAccount(UserEnt entity)
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
                    var data = await context.QueryFirstOrDefaultAsync<UserEnt>("ActivatedAccount",
                        new { entity.User_Id },
                        commandType: CommandType.StoredProcedure);

                    response.Success = true;
                    response.Code = 200;
                    return Ok(response);
                }
            }
            catch (SqlException ex)
            {
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                response.Code = 500;
                return BadRequest(response);
            }
        }
    }
}
