using AdminPanel.Models.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class VendorForCreation
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string PhoneNumber { get; set; }
        public bool Trusted { get; set; }
        public string Email { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime CloseAt { get; set; }
        public string Location { get; set; }
        public byte[] ProfileBytes{ get; set; }
        public byte[] BannerBytes { get; set; }
        public string Description { get; set; }
        public string OwnerId { get; set; }
        public List<CategoryListModel> Categories { get; set; }
    }
}
