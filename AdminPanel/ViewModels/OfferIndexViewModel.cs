using AdminPanel.Models;
using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class OfferIndexViewModel
    {
        public List<Offer> Offers { get; set; } = new List<Offer>();
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public OfferIndexViewModel()
        {

        }


        public OfferIndexViewModel(VendorDomainModel vendor)
        {
            Offers = vendor.Offers;
            VendorId = vendor.Id;
            VendorName = vendor.Name;
        }
        public OfferIndexViewModel(List<Offer> offers, Guid vendorId, string vendorName)
        {
            VendorName = vendorName;
            VendorId = vendorId;
            Offers = offers;
        }
    }
}
