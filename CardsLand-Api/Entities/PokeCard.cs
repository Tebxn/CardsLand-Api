using CardsLand_Api.Entities.CardsProperties;

namespace CardsLand_Api.Entities
{
    public class PokeCard
    {
        public string? CardId { get; set; } //Unique identifier for the object.
        public string? CardName { get; set; }//The name of the card.
        public string? SuperType { get; set; }//The supertype of the card, such as Pokémon, Energy, or Trainer.
        public List<string>? SubTypes { get; set; }//A list of subtypes, such as Basic, EX, Mega, Rapid Strike, etc.
        public string? Level { get; set; }//The level of the card. This only pertains to cards from older sets and those of supertype Pokémon.
        public string? Hp { get; set; }//The hit points of the card.
        public List<string>? Types { get; set; }//The energy types for a card, such as Fire, Water, Grass, etc.
        public string? EvolvesFrom { get; set; }//Which Pokémon this card evolves from.
        public List<string>? EvolvesTo { get; set; }//Which Pokémon this card evolves to. Can be multiple, for example, Eevee.
        public List<string>? Rules { get; set; }//Any rules associated with the card. For example, VMAX rules, Mega rules, or various trainer rules.
        public AncientTrait? AncientTrait { get; set; }//The ancient trait for a given card
        public List<Abilities>? Abilities { get; set; }//One or more abilities for a given card.
        public List<Attacks>? Attacks { get; set; }//One or more attacks for a given card. 
        public List<Weaknesses>? Weaknesses { get; set; }//One or more weaknesses for a given card.
        public List<Resistances>? Resistances { get; set; }//One or more resistances for a given card.
        public List<string>? RetreadCost { get; set; }//A list of costs it takes to retreat and return the card to your bench.
        public int? ConvertedRetreatCost { get; set; }//The converted retreat cost for a card is the count of energy types found within the retreatCost field. For example, ["Fire", "Water"] has a converted retreat cost of 2.
        public Set? Set { get; set; } //The set details embedded into the card
        public string? Number { get; set; }//The number of the card.
        public string? Artist { get; set; }//The artist of the card.
        public string? Rarity { get; set; }//The rarity of the card, such as "Common" or "Rare Rainbow".
        public string? FlavorText { get; set; }//The flavor text of the card. This is the text that can be found on some Pokémon cards that is usually italicized near the bottom of the card.
        public List<int>? NationalPokedexNumbers { get; set; }//The national pokedex numbers associated with any Pokémon featured on a given card.
        public Legalities? Legalities { get; set; }//The legalities for a given card. A legality will not be present in the hash if it is not legal. If it is legal or banned, it will be present.
        public string? RegulationMark { get; set; }//A letter symbol found on each card that identifies whether it is legal to use in tournament play. Regulation marks were introduced on cards in the Sword & Shield Series.
        public Images? Images { get; set; } //The images for a card.
        public TcgPlayer? TcgPlayer { get; set; }//The TCGPlayer information for a given card. ALL PRICES ARE IN US DOLLARS.
        public CardMarket? CardMarked { get; set; }//The cardmarket information for a given card. ALL PRICES ARE IN EUROS.

    }
}
