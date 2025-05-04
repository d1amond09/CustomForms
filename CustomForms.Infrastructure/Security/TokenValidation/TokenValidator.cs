using CustomForms.Infrastructure.Security.TokenGenerator;
using CustomForms.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;

namespace CustomForms.Infrastructure.Security.TokenValidation;

public class TokenValidator(IOptions<JwtConfiguration> jwtConfig, IConfiguration config) : ITokenValidator
{
	private readonly JwtConfiguration _jwtConfig = jwtConfig.Value;
	private readonly IConfiguration _config = config;


	public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
	{
		var tokenValidationParameters = GetTokenValidationParameters();

		var tokenHandler = new JwtSecurityTokenHandler();
		var principal = tokenHandler
			.ValidateToken(
				token,
				tokenValidationParameters,
				out SecurityToken securityToken);

		if (securityToken is not JwtSecurityToken jwtSecurityToken ||
			!jwtSecurityToken.Header.Alg.Equals(
				SecurityAlgorithms.HmacSha256,
				StringComparison.InvariantCultureIgnoreCase)
			)
		{
			throw new SecurityTokenException("Invalid token");
		}

		return principal;
	}

	public TokenValidationParameters GetTokenValidationParameters()
	{
		string? secretKey = _config.GetValue<string>("SECRET");

		ArgumentNullException.ThrowIfNull(secretKey);

		return new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidIssuer = _jwtConfig.ValidIssuer,
			ValidAudience = _jwtConfig.ValidAudience,
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(secretKey)),
		};
	}
}
