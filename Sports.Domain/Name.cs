using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class Name
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Value { get; set; }
        [Required]
        [MaxLength(20)]
        public string Culture { get; set; }
        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
    }
}
