using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace menu_service
{
    public static class AuthorizationHelper
    {
        public static string GetRequestSub(HttpRequest request)
        {
            string authHeader = request.Headers.Authorization;

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadJwtToken(authHeader.Split(' ')[1]);

            Claim? claim = token.Claims.FirstOrDefault(x => x.Type == "sub");

            if (claim == null)
                return "";

            return claim.Value;
        }
    }
}
