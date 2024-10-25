using System.ComponentModel.DataAnnotations;


namespace Logic.Dtos.Clients
{
    public class ClientCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public List<string> Directions { get; set; }
    }
}
