using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public float ReviewValue { get; set; }
        public string Comment { get; set; }
        public bool Updated { get; set; }
        public Guid VendorId { get; set; }
    }
}
