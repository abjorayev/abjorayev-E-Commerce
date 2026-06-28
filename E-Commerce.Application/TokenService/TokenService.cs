using E_Commerce.Domain.Entities;
using E_Commerce.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.TokenService
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IECommerceRepository<RefreshToken> _refreshTokenRepository;
        public TokenService(IConfiguration configuration, IECommerceRepository<RefreshToken> refreshTokenRepository)
        {
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public string GenerateAccessToken(ApplicationUser user)
        {
            var claims = new List<Claim>
        {
            new Claim("UserId", user.Id),
            new Claim("UserName", user?.UserName) 
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshToken(string userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"])),
                IsRevoked = false
            };

            await _refreshTokenRepository.Add(refreshToken);

            return refreshToken.Token;
        }

        public async Task<RefreshToken> GetActiveRefreshToken(string token)
        {
            return await _refreshTokenRepository.Query()
                .FirstOrDefaultAsync(x => x.Token == token && !x.IsRevoked && x.ExpiresAt > DateTime.UtcNow) ?? new RefreshToken();
        }

        public async Task RevokeRefreshToken(string token)
        {
            var refreshToken = await _refreshTokenRepository.Query().FirstOrDefaultAsync(x => x.Token == token);
            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await _refreshTokenRepository.Update(refreshToken);
            }
        }

    }
}
