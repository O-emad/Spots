using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class VendorGalleryForCreation
    {
        [Required]

        public byte[] FileBytes { get; set; }
        [StringLength(50)]
        public string Title { get; set; }
    }
}
