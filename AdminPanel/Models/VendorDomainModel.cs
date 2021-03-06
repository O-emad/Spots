using AdminPanel.Models.Category;
using Spots.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class VendorDomainModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string ProfilePicFileName { get; set; }
        public string BannerPicFileName { get; set; }
        [MaxLength(150)]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        public bool Trusted { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime CloseAt { get; set; }
        public string Location { get; set; }
        public float ReviewAverage { get; set; }
        public int ReviewCount { get; set; }
        public string OwnerId { get; set; }
        public string Description { get; set; }
        public int Follows { get; set; }
        public bool AutoAcceptOffer { get; set; }
        public bool Enabled { get; set; }
        public List<Offer> Offers { get; set; }
        public List<CategoryListModel> Categories { get; set; }
    }
}
