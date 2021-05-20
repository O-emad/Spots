using Microsoft.AspNetCore.Http;
using Spots.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class AdEditAndCreateViewModel
    {
        [Required(ErrorMessage = "Please enter Ad Title")]
        [Display(Name = "Title")]
        [StringLength(50)]
        public string Name { get; set; }
        public Guid Id { get; set; }
        public int SortOrder { get; set; } = 0;
        public string ExternalLink { get; set; }
        public string FileName { get; set; }
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();

        public AdEditAndCreateViewModel()
        {

        }
        public AdEditAndCreateViewModel(Ad ad)
        {
            Name = ad.Name;
            Id = ad.Id;
            SortOrder = ad.SortOrder;
            ExternalLink = ad.ExternalLink;
            FileName = ad.FileName;
        }
    }
}
