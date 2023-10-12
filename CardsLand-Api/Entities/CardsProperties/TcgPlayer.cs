namespace CardsLand_Api.Entities.CardsProperties
{
    public class TcgPlayer
    {
        public string Url { get; set; }//The URL to the TCGPlayer store page to purchase this card.
        public string UpdatedAt { get; set; }//A date that the price was last updated. In the format of YYYY/MM/DD
        public TcgPlayerPrices Prices { get; set; }//A hash of price types. All prices are in US Dollars.
    }
}
