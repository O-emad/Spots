using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class ReviewIndexViewModel
    {
        public List<Review> Reviews { get; set; } = new List<Review>();
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public ReviewIndexViewModel()
        {

        }


        //public ReviewIndexViewModel(VendorDomainModel vendor)
        //{
        //    Offers = vendor.Offers;
        //    VendorId = vendor.Id;
        //    VendorName = vendor.Name;
        //}
        public ReviewIndexViewModel(List<Review> reviews)
        {
            Reviews = reviews;
        }
    }
}
