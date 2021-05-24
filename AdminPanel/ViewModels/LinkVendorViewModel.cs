using AdminPanel.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class LinkVendorViewModel
    {
        public Guid Id { get; set; }
        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();
        public string SelectedUserSubject { get; set; }
        public LinkVendorViewModel()
        { }

        public LinkVendorViewModel(Guid id, IEnumerable<UserModel> usersList)
        {
            Id = id;

            foreach (var item in usersList)
            {
                var selectItem = new SelectListItem { Text = item.UserName, Value = item.Subject };
                Users.Add(selectItem);
            }
            
        }
    }
}
