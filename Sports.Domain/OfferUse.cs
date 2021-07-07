using Spots.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class OfferUse : EntityBase
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Offer")]
        public Guid OfferId { get; set; }
        [Required]
        [StringLength(50)]
        public string UserSubject { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
    }
}
