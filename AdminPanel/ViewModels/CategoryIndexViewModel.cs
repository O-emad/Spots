using AdminPanel.Models.Category;
using Spots.Domain;
using Spots.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class CategoryIndexViewModel
    {
        public IEnumerable<CategoryListModel> Categories { get; private set; }
            = new List<CategoryListModel>();

        public PaginationHeader Pagination { get; set; }
        public CategoryIndexViewModel(IEnumerable<CategoryListModel> categories, PaginationHeader pagination)
        {
            Pagination = pagination;
            Categories = categories;
        }
    }
    
}
