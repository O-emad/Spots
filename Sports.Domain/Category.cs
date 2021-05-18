using System;

namespace Spots.Domain
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NameAR { get; set; }
        public int SortOrder { get; set; }
        public string FileName { get; set; }
        public Guid SuperCategoryId { get; set; }
    }
}
