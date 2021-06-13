using AdminPanel.Models.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Spots.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class VendorEditAndCreateViewModel
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        public string ProfilePicFileName { get; set; }
        public string BannerPicFileName { get; set; }
        public int SortOrder { get; set; } = 0;
        public DateTime OpenAt { get; set; }
        public DateTime CloseAt { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string OwnerId { get; set; }
        public List<IFormFile> ProfileFile { get; set; } = new List<IFormFile>();
        public List<IFormFile> BannerFile { get; set; } = new List<IFormFile>();
        public List<Guid> SelectedCategories { get; set; }
        public List<CategoryListModel> Categories { get; set; }
        public MultiSelectList CategoriesSelectList { get; set; }
        public VendorEditAndCreateViewModel()
        {

        }
        public VendorEditAndCreateViewModel(Vendor vendor, IEnumerable<CategoryListModel> categories)
        {
            Categories = categories.ToList();
            Id = vendor.Id;
            ProfilePicFileName = vendor.ProfilePicFileName;
            BannerPicFileName = vendor.BannerPicFileName;
            SortOrder = vendor.SortOrder;
            Name = vendor.Name;
            OpenAt = vendor.OpenAt;
            CloseAt = vendor.CloseAt;
            Location = vendor.Location;
            OwnerId = vendor.OwnerId;
            Description = vendor.Description;
            SelectedCategories = vendor.Categories.Select(c => c.Id).ToList();
            CategoriesSelectList = new MultiSelectList(categories, "Id", "Name", SelectedCategories);

        }

    }
}
