﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Dtos.Users
{
    public class UserCreateDto: UserCreateUpdateAbstractDto
    {
        

        [Required]
        public string Password { get; set; }
    }
}
