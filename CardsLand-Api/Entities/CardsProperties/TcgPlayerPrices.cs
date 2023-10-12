namespace CardsLand_Api.Entities.CardsProperties
{
    public class TcgPlayerPrices
    {
        public decimal Low { get; set; }//The low price of the card
        public decimal Mid { get; set; }//The mid price of the card
        public decimal High { get; set; }//	The high price of the card
        public decimal Market { get; set; }//The market value of the card. This is usually the best representation of what people are willing to pay.
        public decimal DirectLow { get; set; }//The direct low price of the card

    }
}
