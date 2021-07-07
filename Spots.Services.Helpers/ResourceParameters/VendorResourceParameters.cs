using Spots.Services.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Services.Helpers.ResourceParameters
{
    public class VendorResourceParameters : IndexResourceParameters
    {
        public Guid CategoryId { get; set; }
        public bool Trusted { get; set; }
        public bool Enabled { get; set; }
    }
}
