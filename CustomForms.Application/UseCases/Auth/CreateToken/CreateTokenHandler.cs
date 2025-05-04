using CustomForms.Application.Common.Responses;
using CustomForms.Application.Common.DTOs;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using CustomForms.Domain.Users;
using System.Security.Claims;
using System.Text;
using MediatR;
using CustomForms.Application.Common.Interfaces;

namespace CustomForms.Application.UseCases.Auth.CreateToken;

public class CreateTokenHandler(
	IJwtTokenGenerator tokenGenerator,
	UserManager<User> userManager) : IRequestHandler<CreateTokenUseCase, ApiBaseResponse>
{
	private readonly IJwtTokenGenerator _tokenGenerator = tokenGenerator;
	private readonly UserManager<User> _userManager = userManager;

	public async Task<ApiBaseResponse> Handle(CreateTokenUseCase request, CancellationToken cancellationToken)
	{
		var signingCredentials = _tokenGenerator.GetSigningCredentials();
		var claims = _tokenGenerator.GetClaims(request.User);

		var roles = await _userManager.GetRolesAsync(request.User);
		
		foreach (var role in roles)
		{
			claims.Add(new Claim(ClaimTypes.Role, role));
		}

		var tokenOptions = _tokenGenerator.GenerateTokenOptions(signingCredentials, claims);
		var refreshToken = _tokenGenerator.GenerateRefreshToken();

		request.User.RefreshToken = refreshToken;

		if (request.PopulateExp)
			request.User.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

		await _userManager.UpdateAsync(request.User);

		var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
		var tokenDto = new TokenDto(accessToken, refreshToken);

		return new ApiOkResponse<TokenDto>(tokenDto);
	}

}
