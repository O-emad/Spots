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
        public Guid? VendorId { get; set; }
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
        public SelectList Vendors { get; set; }

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
            VendorId = ad.VendorId;
        }
        /// <summary>
        /// Takes an IEnumerable of items and populate a new SelectList
        /// using the selected dataValueField and dataTextField
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">list of items to be used in a selectlist</param>
        /// <param name="dataValueField">the value field in the list item to be returned for a selected item</param>
        /// <param name="dataTextField">the text field in the list item to be returned for a selected item</param>
        public void PopulateSelectList<T>(IEnumerable<T> list, string dataValueField = "Id", string dataTextField = "Name")
        {
            Vendors = new SelectList(list, dataValueField, dataTextField, VendorId);
        }
    }
}
