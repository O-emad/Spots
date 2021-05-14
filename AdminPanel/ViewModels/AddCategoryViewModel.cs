using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class AddCategoryViewModel
    {
        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; } = 0;
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
        public Guid SuperCategoryId { get; set; } = new Guid();
    }
}
