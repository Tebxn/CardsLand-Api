using CardsLand_Api.Interfaces;

namespace CardsLand_Api.Implementations
{
    public class BCryptHelper : IBCryptHelper
    {
        public string HashPassword(string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }
    }
}
