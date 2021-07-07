using Spots.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class VendorForCreationDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime CloseAt { get; set; }
        [MaxLength(150)]
        public string Location { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(150)]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        public bool Trusted { get; set; }
        public byte[] ProfileBytes { get; set; }
        public byte[] BannerBytes { get; set; }
        public bool AutoAcceptOffer { get; set; }
        public bool Enabled { get; set; }
        //public List<byte[]> GalleryByets { get; set; }
        //public List<string> VideosUrls { get; set; }
        public List<Category> Categories { get; set; }
    }


}
