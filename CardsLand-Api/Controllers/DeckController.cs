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
        [Route("GetAllDecks/{User_Id}")] //User Decks
        public async Task<IActionResult> GetAllDecks()
        {
            ApiResponse<List<DeckEnt>> response = new ApiResponse<List<DeckEnt>>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var decks = await context.QueryAsync<DeckEnt>("GetAllDecks", commandType: CommandType.StoredProcedure);

                    response.Success = true;
                    response.Data = decks.ToList();
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
