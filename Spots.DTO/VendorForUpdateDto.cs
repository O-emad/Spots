using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class VendorForUpdateDto
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string WorkHours { get; set; }
        public string Location { get; set; }
        public List<Category> Categories { get; set; }
    }
}
