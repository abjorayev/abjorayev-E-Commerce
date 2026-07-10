using E_Commerce.Application.DTO;
using E_Commerce.Application.TokenService;
using E_Commerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterLoginDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Login
            };

            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            
            var result = await _userManager.CreateAsync(user, dto.Password);
            
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "User");
          
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(RegisterLoginDTO dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Login);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid credentials");

            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.GenerateRefreshToken(user.Id);

            return Ok(new AuthResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDTO>> Refresh(RefreshTokenDTO dto)
        {
            var storedToken = await _tokenService.GetActiveRefreshToken(dto.RefreshToken);
            if (storedToken == null)
                return Unauthorized("Invalid or expired refresh token");

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
                return Unauthorized();

            
            await _tokenService.RevokeRefreshToken(dto.RefreshToken);

            var newAccessToken = await _tokenService.GenerateAccessToken(user);
            var newRefreshToken = await _tokenService.GenerateRefreshToken(user.Id);

            return Ok(new AuthResponseDTO
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke(RefreshTokenDTO dto)
        {
            await _tokenService.RevokeRefreshToken(dto.RefreshToken);
            return Ok("Token revoked");
        }
    }
}
