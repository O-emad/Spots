using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class VendorGalleryModel
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FileName { get; set; }
        public Guid VendorId { get; set; }
        [StringLength(50)]
        public string Title { get; set; }
    }
}
