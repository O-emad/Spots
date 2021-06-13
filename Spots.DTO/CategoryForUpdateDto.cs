using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class CategoryForUpdateDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(150)]
        public string NameAR { get; set; }
        public int SortOrder { get; set; }
        public byte[] Bytes { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
