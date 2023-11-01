using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Base;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;

namespace CardsLand_Api.Interfaces
{
    public interface IPokemonTcg
    {
        Task<ApiResourceList<PokemonCard>> GetAllCards();
        Task<ApiResourceList<PokemonCard>> GetSpecificCardbyName(string pokemonName);
    }
}
