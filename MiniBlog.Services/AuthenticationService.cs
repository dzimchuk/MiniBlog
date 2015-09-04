using System.Security.Cryptography;
using System.Text;
using MiniBlog.Contracts;

namespace MiniBlog.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration configuration;

        public AuthenticationService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool Authenticate(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                return false;

            if (!userName.Equals(configuration.Find("auth:userName"), System.StringComparison.OrdinalIgnoreCase))
                return false;

            return VerifyPassword(password);
        }

        private bool VerifyPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA1.Create().ComputeHash(bytes);

            var builder = new StringBuilder();
            foreach (var b in hash)
            {
                builder.AppendFormat("{0:x2}", b);
            }

            return builder.ToString().Equals(configuration.Find("auth:passwordHash"), System.StringComparison.OrdinalIgnoreCase);
        }
    }
}