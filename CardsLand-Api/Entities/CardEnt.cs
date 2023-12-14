namespace CardsLand_Api.Entities
{
    public class CardEnt
    {
        public string Card_Id { get; set; } = string.Empty; //example: 
        public string Card_Name { get; set; } = string.Empty;
        public string Card_Image_Url { get; set; } = string.Empty;
        public int? Card_Quantity { get; set; }
    }
}
