using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class Review
    {
        public Guid Id { get; set; }
        public float ReviewValue { get; set; }
        public string Comment { get; set; }
        public bool Updated { get; set; }
        public Guid VendorId { get; set; }
    }
}
