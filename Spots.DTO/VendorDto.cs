using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class VendorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ProfilePicFileName { get; set; }
        public string BannerPicFileName { get; set; }
        public int SortOrder { get; set; }
        public string WorkHours { get; set; }
        public string Location { get; set; }
        public float ReviewAverage { get; set; }
        public int ReviewCount { get; set; }
        public List<Guid> CategoryId { get; set; }
    }
}
