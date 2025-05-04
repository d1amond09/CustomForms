using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using CustomForms.Domain.Users;
using System.Security.Claims;

namespace CustomForms.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
	public string GenerateRefreshToken();
	public JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims);
	public SigningCredentials GetSigningCredentials();
	public List<Claim> GetClaims(User user);
}
