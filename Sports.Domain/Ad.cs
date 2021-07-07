using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class Ad
    {
        [Key]
        public Guid Id { get; set; }


        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public int SortOrder { get; set; }


        [Required]
        [MaxLength(150)]
        public string FileName { get; set; }

        [MaxLength(150)]
        public string ExternalLink { get; set; }
        [ForeignKey("Vendor")]
        public Guid? VendorId { get; set; }
    }
}
