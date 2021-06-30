using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class VendorVideo
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string VideoUrl { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [ForeignKey("Vendor")]
        public Guid VendorId { get; set; }
    }
}
