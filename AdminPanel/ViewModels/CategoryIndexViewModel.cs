using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class CategoryIndexViewModel
    {
        public IEnumerable<Category> Categories { get; private set; }
            = new List<Category>();

        public CategoryIndexViewModel(IEnumerable<Category> categories)
        {
            Categories = categories;
        }
    }
    
}
