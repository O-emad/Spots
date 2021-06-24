using AdminPanel.Models;
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
        [Required(ErrorMessage = "Please enter Vendor name")]
        [Display(Name = "Name")]
        [StringLength(50)]
        public string Name { get; set; }
        public string ProfilePicFileName { get; set; }
        public string BannerPicFileName { get; set; }
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; } = 0;
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(50)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone Number is needed.")]
        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^01[(0-2)|(5)][0-9]{8}$", ErrorMessage = "Invalid Phone number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Trusted")]
        public bool Trusted { get; set; }
        [Display(Name = "Opens At")]
        [DataType(DataType.DateTime)]
        public DateTime OpenAt { get; set; }
        [Display(Name = "Closes At")]
        [DataType(DataType.DateTime)]
        public DateTime CloseAt { get; set; }
        [MaxLength(150)]
        public string Location { get; set; }
        [Display(Name = "Description")]
        [StringLength(500)]
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
        public VendorEditAndCreateViewModel(VendorDomainModel vendor, IEnumerable<CategoryListModel> categories)
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
            Trusted = vendor.Trusted;
            Email = vendor.Email;
            PhoneNumber = vendor.PhoneNumber;
            SelectedCategories = vendor.Categories.Select(c => c.Id).ToList();
            CategoriesSelectList = new MultiSelectList(categories, "Id", "Name", SelectedCategories);

        }

        public VendorEditAndCreateViewModel(IEnumerable<CategoryListModel> categories)
        {
            CategoriesSelectList = new MultiSelectList(categories, "Id", "Name", SelectedCategories);
        }
        /// <summary>
        /// Takes an IEnumerable of items and populate a new MultiSelectList
        /// using the selected dataValueField and dataTextField
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">list of items to be used in a multiselectlist</param>
        /// <param name="dataValueField">the value field in the list item to be returned for a selected item</param>
        /// <param name="dataTextField">the text field in the list item to be returned for a selected item</param>
        public void PopulateSelectList<T>(IEnumerable<T> list, string dataValueField = "Id", string dataTextField = "Name")
        {
            CategoriesSelectList = new MultiSelectList(list, dataValueField, dataTextField, SelectedCategories);
        }

    }

}
