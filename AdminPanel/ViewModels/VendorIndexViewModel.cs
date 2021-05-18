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
        public IEnumerable<Vendor> Vendors { get; private set; }
            = new List<Vendor>();

        public PaginationHeader Pagination { get; set; }
        public VendorIndexViewModel(IEnumerable<Vendor> vendors, PaginationHeader pagination)
        {
            Pagination = pagination;
            Vendors = vendors;
        }
    }
    
}
