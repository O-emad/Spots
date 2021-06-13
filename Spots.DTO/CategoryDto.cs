using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Spots.Domain;
namespace Spots.DTO
{
    public class CategoryDto
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
        public List<CategoryDto> Categories { get; set; }
        public List<VendorDto> Vendors { get; set; }
    }
}
