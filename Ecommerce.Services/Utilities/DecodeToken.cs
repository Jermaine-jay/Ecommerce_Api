using System.Text;

namespace Ecommerce.Services.Utilities
{
    public static class DecodeToken
    {
        public static async Task<(string user, string operation)> DecodeVerificationToken(string validToken)
        {
            var opslen = (validToken.Count() - 84);
            var getstring = validToken.Substring(opslen, 48);
            var getoperation = validToken.Substring(0, opslen);

            var getUserId = Convert.FromBase64String(getstring);
            var operation = Convert.FromBase64String(getoperation);

            string decodedStr = Encoding.ASCII.GetString(getUserId);
            string ops = Encoding.UTF8.GetString(operation);

            return (decodedStr, ops);
        }
    }
}
