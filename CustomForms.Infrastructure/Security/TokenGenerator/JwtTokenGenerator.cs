using CustomForms.Domain.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using CustomForms.Application.Common.Interfaces;

namespace CustomForms.Infrastructure.Security.TokenGenerator;

public class JwtTokenGenerator(
	IOptionsMonitor<JwtConfiguration> options, 
	IConfiguration config) : IJwtTokenGenerator
{
	private readonly IOptionsMonitor<JwtConfiguration> _options = options;
	private readonly IConfiguration _config = config;
	private IConfigurationSection JwtConfig => _config.GetSection("JwtSettings");

	public string GenerateRefreshToken()
	{
		var randomNumber = new byte[32];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}

	public JwtSecurityToken GenerateTokenOptions(
		SigningCredentials signingCredentials,
		List<Claim> claims)
	{
		var tokenOptions = new JwtSecurityToken(
			issuer: JwtConfig["validIssuer"],
			audience: JwtConfig["validAudience"],
			claims: claims,
			expires: DateTime.Now
				.AddMinutes(Convert.ToDouble(JwtConfig["expires"])),
			signingCredentials: signingCredentials
		);

		return tokenOptions;
	}

	public SigningCredentials GetSigningCredentials()
	{
		var secretValue = _config["SECRET"];
		if (string.IsNullOrEmpty(secretValue))
		{
			throw new InvalidOperationException("The SECRET configuration value is missing.");
		}

		var key = Encoding.UTF8.GetBytes(secretValue);
		var secret = new SymmetricSecurityKey(key);
		return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
	}

	public List<Claim> GetClaims(User user)
	{
		var claims = new List<Claim>
		{
			new (ClaimTypes.Name, user.UserName ?? ""),
			new (ClaimTypes.NameIdentifier, user.Id.ToString())
		};

		return claims;
	}
}
