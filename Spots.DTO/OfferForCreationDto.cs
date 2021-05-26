using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class OfferForCreationDto
    {
        public double Value { get; set; }
        public string Title { get; set; }
        public OfferValueType Type { get; set; }
        public string Description { get; set; }
        public byte[] Bytes { get; set; }

    }
}
