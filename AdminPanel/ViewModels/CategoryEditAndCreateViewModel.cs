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
    public class CategoryEditAndCreateViewModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; } = 0;
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
        public Guid SuperCategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public CategoryEditAndCreateViewModel()
        {

        }
        public CategoryEditAndCreateViewModel(Category category)
        {
            Id = category.Id;
            FileName = category.FileName;
            SortOrder = category.SortOrder;
            SuperCategoryId = category.SuperCategoryId;
            Name = category.Name;
        }

    }
}
