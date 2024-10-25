using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class GenericEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        public int? CreatedById { get; set; }
        public int? ModifiedById { get; set; }
        public User CreatedBy { get; set; }
        public User ModifiedBy { get; set;}
    }
}
