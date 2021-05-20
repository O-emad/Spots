using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class AdForUpdateDto
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public byte[] File { get; set; }
        public string ExternalLink { get; set; }

    }
}
