using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class Vendor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string WorkHours { get; set; }
        public string Location { get; set; }
        public List<Offer> Offers { get; set; }

    }
}
