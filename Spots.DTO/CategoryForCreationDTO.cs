using System;

namespace Spots.DTO
{
    public class CategoryForCreationDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid SuperCategoryId { get; set; }
    }
}
