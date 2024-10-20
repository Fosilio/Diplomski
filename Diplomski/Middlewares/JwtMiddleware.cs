using Diplomski.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Diplomski.Middlewares
{
    public class JwtMiddleware : IMiddleware
    {
        private readonly JwtSecurityTokenHandlerWrapper _jwtSecurityTokenHandler;

        public JwtMiddleware(JwtSecurityTokenHandlerWrapper jwtSecurityTokenHandler)
        {
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            if (!token.IsNullOrEmpty())
            {

                try
                {
                    var claimsPrincipal = _jwtSecurityTokenHandler.ValidateJwtToken(token);

                    var username = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    context.Items["NameIdentifier"] = username;
                }
                catch (Exception)
                {
                    context.Response.StatusCode = new UnauthorizedResult().StatusCode;
                }


            }
            await next(context);
        }
    }
}
