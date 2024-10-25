using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class PasswordRecoveryCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime Expires { get; set; } = DateTime.Now.AddHours(3);
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
