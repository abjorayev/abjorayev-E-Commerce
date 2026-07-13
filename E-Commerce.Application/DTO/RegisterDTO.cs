using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.DTO
{
    public class RegisterLoginDTO
    {
        [Required(ErrorMessage = "Login is required")]
        [MinLength(3, ErrorMessage = "Login must be at least 3 characters")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

    }

    public class RefreshTokenDTO
    {
        public string RefreshToken { get; set; }
    }
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
