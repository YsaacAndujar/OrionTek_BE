using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Client : GenericEntity
    {
        [Required]
        public string Name { get; set; }
        public List<Direction> Directions { get; set; }
    }
}
