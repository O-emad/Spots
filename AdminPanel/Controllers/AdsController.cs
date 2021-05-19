using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    [Authorize]
    public class AdsController : Controller
    {
        public IActionResult Index()
        {
            return View(new VendorEditAndCreateViewModel(new Vendor()
            {
                Name = "3y4",

            },
            new List<Category>()
            {
                new Category()
                {
                    Id = new Guid(),
                    Name = "Electronics"
                },
                new Category()
                {
                    Id = new Guid(),
                    Name = "Electronics 1"
                },
                new Category()
                {
                    Id = new Guid(),
                    Name = "Electronics 2"
                }
            }));
        }
    }
}
