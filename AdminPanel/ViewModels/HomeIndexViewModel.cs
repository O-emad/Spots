using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class HomeIndexViewModel
    {
        public int AdsCount { get; set; }
        public int CategoriesCount { get; set; }
        public int VendorsCount { get; set; }
        public int FollowsCount { get; set; }
        public List<OfferAcceptanceModel> PendingOffers { get; set; }

        public HomeIndexViewModel()
        {

        }

    }

    public class OfferAcceptanceModel
    {
        public Guid OfferId { get; set; }
        public string OfferTitle { get; set; }
        public Guid VendorId { get; set; }
        public String VendorName { get; set; }
    }
}
