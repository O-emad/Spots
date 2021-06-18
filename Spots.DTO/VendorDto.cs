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
        public DateTime OpenAt { get; set; }
        public DateTime CloseAt { get; set; }
        public string Location { get; set; }
        public float ReviewAverage { get; set; }
        public int ReviewCount { get; set; }
        public string OwnerId { get; set; }
        public string Description { get; set; }
        public int Follows { get; set; }
        public List<Offer> Offers { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }
}
