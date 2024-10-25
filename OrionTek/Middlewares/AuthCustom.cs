using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Text;

namespace OrionTek.Middlewares
{
    public class AuthCustom : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var env = context.HttpContext.RequestServices
             .GetRequiredService<IWebHostEnvironment>();
            var allowAnonymous = context.ActionDescriptor
                                  .EndpointMetadata
                                  .Any(em => em.GetType() == typeof(AllowAnonymousAttribute));

            if (allowAnonymous)
            {
                return;
            }

            var token = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var configuration = context.HttpContext.RequestServices
            .GetRequiredService<IConfiguration>();
            var key = Encoding.ASCII.GetBytes(configuration["keyJwt"]);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken validatedToken;
            try
            {
                tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var userEmail = (tokenHandler.ReadToken(token) as JwtSecurityToken)?.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            var repository = context.HttpContext.RequestServices
            .GetRequiredService<IAuthenticationRepository>();
            var user = await repository.GetUserFromEmail(userEmail);
            if (user == null)
            {
                context.Result = new StatusCodeResult(401);
                return;
            }
            context.HttpContext.Items["AuthenticatedUser"] = user;
        }
    }
}
