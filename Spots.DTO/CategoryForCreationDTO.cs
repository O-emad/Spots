using System;
using System.ComponentModel.DataAnnotations;

namespace Spots.DTO
{
    public class CategoryForCreationDto
    {
        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public byte[] Bytes { get; set; }
        public Guid SuperCategoryId { get; set; }
    }
}
