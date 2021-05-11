using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
