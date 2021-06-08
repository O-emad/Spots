using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class Follow
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Vendor")]
        public Guid VendorId { get; set; }

        public Guid UserId { get; set; }
    }
}
