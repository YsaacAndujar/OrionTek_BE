using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Dtos.Authentication
{
    public class ChangePasswordDto
    {
        [Required]
        public string Password { get; set; }
    }
}
