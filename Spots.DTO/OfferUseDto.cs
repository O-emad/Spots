using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class OfferUseDto
    {
        public Guid Id { get; set; }
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
