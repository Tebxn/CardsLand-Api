using CardsLand_Api.Interfaces;
using System.Net.Mail;
using System.Text;

namespace CardsLand_Api.Implementations
{
    public class Tools : ITools
    {
        private readonly IConfiguration _configuration;

        public Tools(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public void SendEmail(string recipient, string subject, string body)
        {
            //Usar el metodo del profe
        }
    }
}

