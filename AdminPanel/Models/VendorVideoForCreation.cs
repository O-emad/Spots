using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class VendorVideoForCreation
    {
        [Required]
        [MaxLength(250)]
        public string VideoUrl { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
    }
}
