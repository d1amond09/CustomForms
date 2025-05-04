using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace CustomForms.Application.Common.Interfaces;

public interface ITokenValidator
{
	ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
	TokenValidationParameters GetTokenValidationParameters();
}
