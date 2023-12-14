using CardsLand_Api.Entities;
using System.Security.Claims;

namespace CardsLand_Api.Interfaces
{
    public interface ITools
    {
        String GenerateRandomCode(int length);
        bool SendEmail(string recipient, string subject, string body);
        string Decrypt(string texto);
        string Encrypt(string texto);
        string GenerateToken(string userId, string userIsAdmin);
        bool CheckPassword(string password, string hashedPassword);
        public void ObtainClaims(IEnumerable<Claim> values, ref string userId, ref string userIsAdmin, ref bool isAdmin);
        public void ObtainClaimsID(IEnumerable<Claim> values, ref string userId);
        public string MakeHtmlNewUser(UserEnt userData, string temporalPassword);
        public string MakeHtmlPassRecovery(UserEnt userData, string temporalPassword);
        Task AddError(string errorMessage);
    }
}
