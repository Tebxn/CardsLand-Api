namespace CardsLand_Api.Interfaces
{
    public interface ITools
    {
        String GenerateRandomCode(int length);
        bool SendEmail(string recipient, string subject, string body);
        string Decrypt(string texto);
        string Encrypt(string texto);
        string GenerateToken(string userId);
        string MakeHtmlNewUser(string nickname, string activationCode);
    }
}
