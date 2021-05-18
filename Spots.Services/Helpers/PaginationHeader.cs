using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Services.Helpers
{
    public class PaginationHeader
    {
        public int currentPage { get; set; }
        public int totalPages { get; set; }
        public string searchQuery { get; set; }
        public string previousPageLink { get; set; }
        public string nextPageLink { get; set; }
    }
}
