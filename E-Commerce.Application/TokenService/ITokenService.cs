using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.TokenService
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(ApplicationUser user);
        Task<string> GenerateRefreshToken(string userId);
        Task<RefreshToken> GetActiveRefreshToken(string token);
        Task RevokeRefreshToken(string token);
    }

}
