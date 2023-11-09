namespace CardsLand_Api.Entities
{
    public class DeckEnt
    {
        public long DeckId { get; set; }
        public string DeckName { get; set; } = string.Empty;
        public string DeckDescription { get; set; } = string.Empty;
        public string DeckBackgroundImage { get; set; } = string.Empty;
    }
}
