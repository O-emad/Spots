using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class Vendor
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string ProfilePicFileName { get; set; }
        [MaxLength(50)]
        public string BannerPicFileName { get; set; }
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

        public bool AutoAcceptOffer { get; set; }
        public bool Enabled { get; set; }

        public string PhoneNumber { get; set; }
        public bool Trusted { get; set; }
        public bool HasOffer { get; set; }
        public List<Offer> Offers { get; set; }
        public List<Category> Categories { get; set; }
        public List<Follow> Follows { get; set; }
        public List<VendorGallery> GalleryFileNames { get; set; }
        public List<VendorVideo> VideosUrls { get; set; }
        public List<Ad> Ads { get; set; }


    }
}
