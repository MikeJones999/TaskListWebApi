using System.Security.Cryptography;

namespace TaskList.Api.Utility.StaticHelpers
{
    public static class RefreshTokenHelper
    {
        public static string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using (RandomNumberGenerator randNumGen = RandomNumberGenerator.Create())
            {
                randNumGen.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }

        }
    }
}
