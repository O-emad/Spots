﻿using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class VendorForUpdateDto
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string WorkHours { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime CloseAt { get; set; }
        public string Location { get; set; }
        public byte[] ProfileBytes { get; set; }
        public byte[] BannerBytes { get; set; }
        public string OwnerId { get; set; }
        public string Description { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }
}
