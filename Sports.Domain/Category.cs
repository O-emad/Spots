﻿using System;

namespace Spots.Domain
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid SuperCategoryId { get; set; }
    }
}
