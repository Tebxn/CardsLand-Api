namespace CardsLand_Api.Entities.CardsProperties
{
    public class Set //La expansion que salio la carta
    {
        public string? Id { get; set; }
        public string? Name { get; set; }//The name of the set.
        public string? Series { get; set; }//The series the set belongs to, like Sword and Shield or Base.
        public int PrintedTotal { get; set; }//The number printed on the card that represents the total. This total does not include secret rares.
        public int Total { get; set; } //The total number of cards in the set, including secret rares, alternate art, etc.
        public Legalities? Legalities { get; set; }//The legalities of the set. If a given format is not legal, it will not appear in the hash.
        public string? PtcgoCode { get; set; } //The code the Pokémon Trading Card Game Online uses to identify a set.
        public string? ReleaseDate { get; set; }//The date the set was released (in the USA). Format is YYYY/MM/DD.
        public string? UpdatedAt { get; set; }//The date and time the set was updated. Format is YYYY/MM/DD HH:MM:SS.
        public Images? Images { get; set; } //Any images associated with the set, such as symbol and logo

    }
}
