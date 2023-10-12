namespace CardsLand_Api.Entities.CardsProperties
{
    public class Attacks
    {
        public List<string> Cost { get; set; }//The cost of the attack represented by a list of energy types.
        public string Name { get; set; }//The name of the attack
        public string Text { get; set; }//The text or description associated with the attack
        public string Damage { get; set; }//The damage amount of the attack
        public int ConvertedEnergyCost { get; set; }//The total cost of the attack. For example, if it costs 2 fire energy, the converted energy cost is simply 2.

    }
}
