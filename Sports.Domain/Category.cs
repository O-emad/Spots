﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spots.Domain
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NameAR { get; set; }
        public int SortOrder { get; set; }
        public string FileName { get; set; }
        //public Guid SuperCategoryId { get; set; }
        public Guid SuperCategoryId { get; set; }
        public List<Category> SubCategories { get; set; } = new List<Category>();
        public List<Vendor> Vendors { get; set; }
    }
}
