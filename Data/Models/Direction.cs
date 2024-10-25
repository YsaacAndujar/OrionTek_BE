
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Direction
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
