namespace CardsLand_Api.Entities
{
    public class DeckEnt
    {
        public long Deck_Id { get; set; }
        public long Deck_User_Id { get; set; } //fk to users
        public string Deck_Name { get; set; } = string.Empty;
        public string Deck_Description { get; set; } = string.Empty;
        public string Deck_Background_Image { get; set; } = string.Empty;
    }
}
