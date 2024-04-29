using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistance.SharedServices
{
    public static class Authorization
    {
        public static int GetUserIdFromJWT(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "uid");
                if (userId != null && string.IsNullOrEmpty(userId.Value) == false)
                {
                    return Convert.ToInt32(userId.Value);
                }
            }
            catch (Exception)
            { }

            return 0;
        }

    }
}
