using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class VendorForCreation
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string WorkHours { get; set; }
        public string Location { get; set; }
        public byte[] ProfileBytes{ get; set; }
        public byte[] BannerBytes { get; set; }
    }
}
