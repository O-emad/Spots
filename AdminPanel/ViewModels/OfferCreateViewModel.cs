using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class OfferCreateViewModel
    {
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public OfferForCreation Offer { get; set; }
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();

        public List<SelectListItem> SelectList { get; set; } = new List<SelectListItem>()
        {
            new SelectListItem(OfferValueType.Percentage.ToString(), OfferValueType.Percentage.ToString("D")),
            new SelectListItem(OfferValueType.Value.ToString(), OfferValueType.Value.ToString("D"))
        };

        public OfferCreateViewModel()
        {

        }
        public OfferCreateViewModel(Guid vendorid, string vendorName)
        {
            VendorName = vendorName;
            VendorId = vendorid;
        }
    }
}
