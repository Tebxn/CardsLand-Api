using CardsLand_Api.Entities;
using CardsLand_Api.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using CardsLand_Api.Interfaces;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Base;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;

namespace CardsLand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonTcgController : ControllerBase
    {
        private readonly IPokemonTcg _pokemonTcg;

        public PokemonTcgController(IPokemonTcg pokemonTcg)
        {
            _pokemonTcg = pokemonTcg;
        }

        [HttpGet]
        [Route("GetAllCards")]
        public async Task<IActionResult> GetAllCards()
        {
            ApiResponse<ApiResourceList<PokemonCard>> response = new ApiResponse<ApiResourceList<PokemonCard>>();

            try
            {
                response.Data = await _pokemonTcg.GetAllCards();
                return Ok(response);
            }
            catch (SqlException ex)
            {
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                response.Code = 500;
                return BadRequest(response);
            }
        }

        [HttpGet]
        [Route("GetSpecificCardbyName")]
        public async Task<IActionResult> GetSpecificCardbyName(string cardName)
        {
            ApiResponse<ApiResourceList<PokemonCard>> response = new ApiResponse<ApiResourceList<PokemonCard>>();

            try
            {
                response.Data = await _pokemonTcg.GetSpecificCardbyName(cardName);
                return Ok(response);
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
