using AdminPanel.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class VendorMediaViewModel
    {
        public List<VendorGalleryModel> Gallery { get; set; }
        public List<VendorVideoModel> Video { get; set; }
        public IFormFile GalleryFile { get; set; }
        [Display(Name = "Video Url")]
        [Url(ErrorMessage = "Enter a valid Url")]
        [StringLength(250)]
        public string VideoUrl { get; set; }
        [Display(Name = "Video Title")]
        [StringLength(50)]
        public string VideoTitle { get; set; }
        [Display(Name = "Image Title")]
        [StringLength(50)]
        public string GalleryTitle { get; set; }

        public VendorMediaViewModel()
        {

        }
        public VendorMediaViewModel(List<VendorGalleryModel> gallery, List<VendorVideoModel> video)
        {
            Gallery = gallery;
            Video = video;
        }
    }
}
