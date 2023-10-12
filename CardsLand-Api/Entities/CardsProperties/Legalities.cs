namespace CardsLand_Api.Entities.CardsProperties
{
    public class Legalities
    {
        public string Standard { get; set; }//The legality ruling for Standard. Can be either Legal, Banned, or not present.
        public string Expanded { get; set; }//The legality ruling for Expanded. Can be either Legal, Banned, or not present.
        public string Unlimited { get; set; }//The legality ruling for Unlimited. Can be either Legal, Banned, or not present.
    }
}
