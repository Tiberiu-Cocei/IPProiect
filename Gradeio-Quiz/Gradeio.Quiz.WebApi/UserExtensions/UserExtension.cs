using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace Gradeio.Quiz.WebApi.UserExtensions
{
    public class UserExtension : IUserExtension
    {
        public Guid GetUserIdFromClaims(HttpContext httpContext)
        {
            var claims = (httpContext.User.Identity as ClaimsIdentity).Claims;
            var user = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            return new Guid(user);
        }
    }
}
