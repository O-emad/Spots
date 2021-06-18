using AdminPanel.Models;
using Spots.Domain;
using Spots.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class VendorIndexViewModel
    {
        public IEnumerable<VendorDomainModel> Vendors { get; private set; }
            = new List<VendorDomainModel>();

        public PaginationHeader Pagination { get; set; }
        public VendorIndexViewModel(IEnumerable<VendorDomainModel> vendors, PaginationHeader pagination)
        {
            Pagination = pagination;
            Vendors = vendors;
        }
    }
    
}
