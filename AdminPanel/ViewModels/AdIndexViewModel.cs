using Spots.Domain;
using Spots.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class AdIndexViewModel
    {
        public IEnumerable<Ad> Ads { get; private set; }
            = new List<Ad>();

        public PaginationHeader Pagination { get; set; }
        public AdIndexViewModel(IEnumerable<Ad> ads, PaginationHeader pagination)
        {
            Pagination = pagination;
            Ads = ads;
        }
    }
}
