using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class CategoryEditViewModel
    {
        public Category Category { get; set; } = new Category();
        public CategoryEditViewModel(Category category)
        {
            Category = category;
        }
    }
}
