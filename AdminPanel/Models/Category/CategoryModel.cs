using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Models.Category
{
    public class CategoryModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        public int SortOrder { get; set; }
        [MaxLength(150)]
        public string FileName { get; set; }
        public Guid? CategoryId { get; set; }
        public List<CategoryModel> Categories { get; set; }
    }
}
