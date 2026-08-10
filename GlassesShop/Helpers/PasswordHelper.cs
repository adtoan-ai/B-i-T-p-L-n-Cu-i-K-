using System.Security.Cryptography;
using System.Text;

namespace GlassesShop.Helpers
{
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password + "GlassesShop@2026"));
            return Convert.ToBase64String(bytes);
        }

        public static bool Verify(string password, string hashed)
        {
            return Hash(password) == hashed;
        }
    }
}