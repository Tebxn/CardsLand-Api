using CardsLand_Api.Entities;
using CardsLand_Api.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace CardsLand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeckController : ControllerBase
    {
        private readonly IDbConnectionProvider _connectionProvider;
        private readonly ITools _tools;

        public DeckController(IDbConnectionProvider connectionProvider, ITools tools)
        {
            _connectionProvider = connectionProvider;
            _tools = tools;
        }

        [HttpGet]
        [Authorize]
        [Route("GetAllUserDecks/{userId}")] //User Decks
        public async Task<IActionResult> GetAllUserDecks(string userId)
        {
            ApiResponse<List<DeckEnt>> response = new ApiResponse<List<DeckEnt>>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {

                    var data = await context.QueryAsync<DeckEnt>("GetAllUserDecks",
                    new { User_Id = userId }, commandType: CommandType.StoredProcedure);

                    response.Success = true;
                    response.Data = data.ToList();
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

        [HttpPost]
        [Authorize]
        [Route("CreateDeck")]
        public async Task<IActionResult> CreateDeck(DeckEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {

                    var data = await context.ExecuteAsync("CreateDeck",
                    new
                    {
                        User_Id = entity.Deck_User_Id,
                        Deck_Name = entity.Deck_Name,
                        Deck_Description = entity.Deck_Description,
                        Deck_BackgroundImage = entity.Deck_Background_Image
                    },
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

            }
            catch (SqlException ex)
            {
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                response.Code = 500;
                return BadRequest(response);
            }
        }
        [HttpDelete]
        [Authorize]
        [Route("DeleteDeck")]
        public async Task<IActionResult> DeleteDeck(DeckEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {

                    var data = await context.ExecuteAsync("DeleteDeck",
                    new
                    {
                        Deck_Id = entity.Deck_Id
                    },
                    commandType: CommandType.StoredProcedure);

                    if (data != 0)
                    {
                        response.Success = true;
                        response.Code = 200;
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "Server Error";
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
    }
}
