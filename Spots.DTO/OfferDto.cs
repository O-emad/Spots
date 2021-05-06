using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class OfferDto
    {
        public Guid Id { get; set; }
        public double Value { get; set; }
        public ValueType Type { get; set; }
        public string Description { get; set; }
        public bool OfferApproved { get; set; }
        public Guid VendorId { get; set; }
    }
}
