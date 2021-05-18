using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spots.Domain;
namespace Spots.DTO
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NameAR { get; set; }
        public int SortOrder { get; set; }
        public string FileName { get; set; }
        public Guid SuperCategoryId { get; set; }
    }
}
