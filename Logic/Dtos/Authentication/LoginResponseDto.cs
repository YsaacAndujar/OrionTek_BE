using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Dtos.Authentication
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
