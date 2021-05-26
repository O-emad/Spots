using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class Offer
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public double Value { get; set; }
        public OfferValueType Type { get; set; }
        public string Description { get; set; }
        public bool OfferApproved { get; set; }
        public string FileName { get; set; }
        public Guid VendorId { get; set; }

    }

    public enum OfferValueType
    {
        Value = 0,
        Percentage
    }
}
