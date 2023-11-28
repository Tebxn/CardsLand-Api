using CardsLand_Api.Entities;
using CardsLand_Api.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using CardsLand_Api.Interfaces;
using Dapper;

namespace CardsLand_Api.Controllers

{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : Controller
    {
        private readonly IDbConnectionProvider _connectionProvider;
        private readonly ITools _tools;

        public UserController(IDbConnectionProvider connectionProvider, ITools tools)
        {
            _connectionProvider = connectionProvider;
            _tools = tools;
        }

        [HttpGet]
        [Authorize]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            ApiResponse<List<UserEnt>> response = new ApiResponse<List<UserEnt>>();

            try
            {
                string userTokenId = string.Empty;
                string userIsAdmin = string.Empty;
                bool isAdmin = false;
                _tools.ObtainClaims(User.Claims, ref userTokenId, ref userIsAdmin, ref isAdmin);

                if (!isAdmin)
                    return Unauthorized();

                using (var context = _connectionProvider.GetConnection())
                {
                    var users = await context.QueryAsync<UserEnt>("GetAllUsers", commandType: CommandType.StoredProcedure);

                    response.Success = true;
                    response.Data = users.ToList();
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

        [HttpGet]
        [AllowAnonymous]
        [Route("GetSpecificUser/{userId}")]
        public async Task<IActionResult> GetSpecificUser(string userId)
        {
            ApiResponse<UserEnt> response = new ApiResponse<UserEnt>();

            try
            {
                string userTokenId = string.Empty;
                string userIsAdmin = string.Empty;
                bool isAdmin = false;
                _tools.ObtainClaims(User.Claims, ref userTokenId, ref userIsAdmin, ref isAdmin);

                if (!isAdmin)
                    return Unauthorized();

                // Desencripta el valor de userId para obtener UserId
                string decryptedUserId = _tools.Decrypt(userId);

                if (long.TryParse(decryptedUserId, out long parsedUserId))
                {
                    using (var context = _connectionProvider.GetConnection())
                    {
                        var user = await context.QueryFirstOrDefaultAsync<UserEnt>("GetSpecificUser", new { UserId = parsedUserId }, commandType: CommandType.StoredProcedure);

                        if (user != null)
                        {
                            response.Success = true;
                            response.Data = user;
                            return Ok(response);
                        }
                        else
                        {
                            response.ErrorMessage = "User not found";
                            response.Code = 404;
                            return NotFound(response);
                        }
                    }
                }
                else
                {
                    response.ErrorMessage = "Invalid UserId";
                    response.Code = 400;
                    return BadRequest(response);
                }
            }
            catch (SqlException ex)
            {
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                response.Code = 500;
                return BadRequest(response);
            }
        }
        [HttpGet]
        [Authorize]
        [Route("GetSpecificUserFromToken")]
        public async Task<IActionResult> GetSpecificUserFromToken()
        {
            ApiResponse<UserEnt> response = new ApiResponse<UserEnt>();

            try
            {
                string userId = string.Empty;


                //Desencripta el valor de userId para obtener UserId. Se borró lo siguiente:  string decryptedUserId = _tools.Decrypt(userToken);
                _tools.ObtainClaimsID(User.Claims, ref userId);

                string decryptedUserId = userId;

                if (long.TryParse(decryptedUserId, out long parsedUserId))
                {
                    using (var context = _connectionProvider.GetConnection())
                    {
                        var user = await context.QueryFirstOrDefaultAsync<UserEnt>("GetSpecificUserFromToken", new { User_Id = parsedUserId }, commandType: CommandType.StoredProcedure);

                        if (user != null)
                        {
                            response.Success = true;
                            response.Data = user;
                            return Ok(response);
                        }
                        else
                        {
                            response.ErrorMessage = "User not found";
                            response.Code = 404;
                            return NotFound(response);
                        }
                    }
                }
                else
                {
                    response.ErrorMessage = "Invalid UserId";
                    response.Code = 400;
                    return BadRequest(response);
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
        [Authorize]
        [Route("EditSpecificUser")]
        public async Task<IActionResult> EditSpecificUser(UserEnt entity)
        {
            ApiResponse<UserEnt> response = new ApiResponse<UserEnt>();

            try
            {
                string userTokenId = string.Empty;
                string userIsAdmin = string.Empty;
                bool isAdmin = false;
                _tools.ObtainClaims(User.Claims, ref userTokenId, ref userIsAdmin, ref isAdmin);

                if (!isAdmin)
                    return Unauthorized();

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("EditSpecificUser",
                        new {
                            entity.User_Id,
                            entity.User_Nickname,
                            entity.User_Email,
                            entity.User_IsAdmin
                        },
                        commandType: CommandType.StoredProcedure);

                    if (data > 0)
                    {
                        response.Success = true;
                        response.Code = 200;
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "User not found";
                        response.Code = 404;
                        return NotFound(response);
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
    }
}
