using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class AdDto
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string FileName { get; set; }
        public string ExternalLink { get; set; }
    }
}
