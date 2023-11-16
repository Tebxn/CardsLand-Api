namespace CardsLand_Api.Entities
{
    public class UserEnt
    {
        public long User_Id { get; set; }
        public string User_Nickname { get; set; } = string.Empty; //unique
        public string User_Email { get; set; } = string.Empty; //unique
        public string User_Password { get; set; } = string.Empty;
        public bool User_IsAdmin { get; set; }
        public bool User_State { get; set; }
        public string? User_Activation_Code { get; set; }
        public bool User_Need_AC { get; set; }
        public string UserToken { get; set; }
    }
}
