using AdminPanel.Models;
using Spots.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class UserIndexViewModel
    {
        public List<UserModel> Users { get; set; }
        = new List<UserModel>();

        public PaginationHeader Pagination { get; set; }

        public UserIndexViewModel(IEnumerable<UserModel> users, PaginationHeader pagination)
        {
            Users = users.ToList();
            Pagination = pagination;
        }
    }
}
