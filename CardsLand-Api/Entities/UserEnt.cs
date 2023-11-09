namespace CardsLand_Api.Entities
{
    public class UserEnt
    {
        public long User_Id { get; set; }
        public string User_Nickname { get; set; } = string.Empty; //unique
        public string User_Email { get; set; } = string.Empty; //unique
        public string User_Password { get; set; } = string.Empty;
        public int User_Type { get; set; }
        public bool User_State { get; set; }
        public bool User_Password_IsTemp { get; set; }
    }
}
