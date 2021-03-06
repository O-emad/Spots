using Spots.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class VendorDto
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string ProfilePicFileName { get; set; }
        [MaxLength(50)]
        public string BannerPicFileName { get; set; }
        //public List<string> GalleryFileNames { get; set; }
        //public List<string> VideosUrls { get; set; }
        public int SortOrder { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime CloseAt { get; set; }
        [MaxLength(150)]
        public string Location { get; set; }
        public float ReviewAverage { get; set; }
        public int ReviewCount { get; set; }
        [MaxLength(50)]
        public string OwnerId { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(150)]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        public bool Trusted { get; set; }
        public bool HasOffer { get; set; }
        public int Follows { get; set; }
        public bool AutoAcceptOffer { get; set; }
        public bool Enabled { get; set; }
        public List<Offer> Offers { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }
}
