using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class AdForCreation
    {

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public int SortOrder { get; set; }
        [Required]
        public byte[] File { get; set; }
        [MaxLength(150)]
        public string ExternalLink { get; set; }
    }
}
