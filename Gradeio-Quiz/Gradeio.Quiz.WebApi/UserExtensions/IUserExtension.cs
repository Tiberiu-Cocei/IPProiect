using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Gradeio.Quiz.WebApi.UserExtensions
{
    public interface IUserExtension
    {
        Guid GetUserIdFromClaims(HttpContext httpContext);
    }
}
