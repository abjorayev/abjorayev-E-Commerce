using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.DTO
{
    public class RegisterLoginDTO
    {
        public string Login { get; set; }
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
