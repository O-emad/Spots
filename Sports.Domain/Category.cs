using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spots.Domain
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(150)]
        public string NameAR { get; set; }
        public int SortOrder { get; set; }
        [MaxLength(100)]
        public string FileName { get; set; }

        public List<Vendor> Vendors { get; set; }


        [ForeignKey("Category")]
        public Guid? CategoryId { get; set; }
        public List<Category> Categories { get; set; }

    }
}
