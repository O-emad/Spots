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
        public int SortOrder { get; set; }
        [MaxLength(100)]
        public string FileName { get; set; }

        public List<Name> Names { get; set; }

        public List<Vendor> Vendors { get; set; }


        [ForeignKey("Category")]
        public Guid? CategoryId { get; set; }
        public List<Category> Categories { get; set; }

    }
}
