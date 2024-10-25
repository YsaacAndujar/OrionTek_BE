using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Dtos.Clients
{
    public class ClientDto: GenericDto
    {
        public string Name { get; set; }
        public List<string> Directions { get; set; }
    }
}
