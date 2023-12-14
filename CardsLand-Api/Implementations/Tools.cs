using CardsLand_Api.Entities;
using CardsLand_Api.Interfaces;
using MimeKit;
using System.Text;
using MailKit.Net.Smtp;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web;
using static Dapper.SqlMapper;
using System.Data;

namespace CardsLand_Api.Implementations
{
    public class Tools : ITools
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnectionProvider _connectionProvider;

        private IHostEnvironment _hostingEnvironment;

        public Tools(IConfiguration configuration, IHostEnvironment hostingEnvironment, IDbConnectionProvider connectionProvider)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _connectionProvider = connectionProvider;
        }

        public string GenerateRandomCode(int length)
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

        public bool SendEmail(string recipient, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                string emailSender = _configuration["Email:SenderAddress"];
                string emailSenderPassword = _configuration["Email:SenderPassword"];
                message.From.Add(new MailboxAddress("CardsLand", emailSender));
                message.To.Add(new MailboxAddress("Recipient", recipient));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = body;

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.office365.com", 587, false);
                    client.Authenticate(emailSender, emailSenderPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public string GenerateToken(string userId, string userIsAdmin)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("userId", Encrypt(userId)));
            claims.Add(new Claim("userIsAdmin", Encrypt(userIsAdmin)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("8Tc2nR3QBamz1ipE3b9aYSiTPYoGXQsy"));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void ObtainClaims(IEnumerable<Claim> values, ref string userId, ref string userIsAdmin, ref bool isAdmin)
        {
            var claims = values.Select(Claim => new { Claim.Type, Claim.Value }).ToArray();
            userId = Decrypt(claims.Where(x => x.Type == "userId").ToList().FirstOrDefault().Value);
            userIsAdmin = Decrypt(claims.Where(x => x.Type == "userIsAdmin").ToList().FirstOrDefault().Value);

            if (userIsAdmin == "True")
                isAdmin = true;
        }

        public void ObtainClaimsID(IEnumerable<Claim> values, ref string userId)
        {
            var claims = values.Select(Claim => new { Claim.Type, Claim.Value }).ToArray();
            userId = Decrypt(claims.Where(x => x.Type == "userId").ToList().FirstOrDefault().Value);
        }

        public string Encrypt(string texto)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes("x3nbTRq6Jqec3lIZ");
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(texto);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }

        public string Decrypt(string texto)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(texto);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes("x3nbTRq6Jqec3lIZ");
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string MakeHtmlPassRecovery(UserEnt userData, string temporalPassword)
        {
            try
            {
                string fileRoute = Path.Combine(_hostingEnvironment.ContentRootPath, "HtmlTemplates\\Contrasenna.html");
                string htmlFile = System.IO.File.ReadAllText(fileRoute);
                htmlFile = htmlFile.Replace("@@Nickname", userData.User_Nickname);
                htmlFile = htmlFile.Replace("@@TemporalPassword", temporalPassword);
                string hashedId = Encrypt(userData.User_Id.ToString());
                string encodedHashedId = HttpUtility.UrlEncode(hashedId);
                htmlFile = htmlFile.Replace("@@Link", "https://localhost:7009/Authentication/UpdateNewPassword?q=" + encodedHashedId);


                return htmlFile;
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        public string MakeHtmlNewUser(UserEnt userData, string activationCode)
        {
            try
            {
                string fileRoute = Path.Combine(_hostingEnvironment.ContentRootPath, "HtmlTemplates\\activationCode.html");
                string htmlFile = System.IO.File.ReadAllText(fileRoute);
                htmlFile = htmlFile.Replace("@@nickname", userData.User_Nickname);
                htmlFile = htmlFile.Replace("@@activationCode", activationCode);
                string hashedId = Encrypt(userData.User_Id.ToString());
                string encodedHashedId = HttpUtility.UrlEncode(hashedId);
                htmlFile = htmlFile.Replace("@@Link", "https://localhost:7009/Authentication/ActivateAccount?q=" + encodedHashedId);

                return htmlFile;
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        public bool CheckPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
            var unHashedPassword = Decrypt(hashedPassword);
            bool validPassword = password == unHashedPassword ? true : false;
            return validPassword;
        }

        public async Task AddError(string errorMessage)
        {
            using (var context = _connectionProvider.GetConnection())
            {
                DateTime dateTime = DateTime.Now;
                var data = await context.ExecuteAsync("AddError",
                new
                {
                    ErrorDate = dateTime,
                    ErrorMessage = errorMessage
                },
                commandType: CommandType.StoredProcedure);
            }
        }
    }
}


