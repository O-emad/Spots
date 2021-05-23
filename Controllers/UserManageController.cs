//using Marvin.IDP.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ExtraSW.IDP.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize(Roles = "Admin")]
//    public class UserManageController : ControllerBase
//    {
//        private readonly ILocalUserService localUserService;

//        public UserManageController(ILocalUserService localUserService)
//        {
//            this.localUserService = localUserService ?? throw new ArgumentNullException(nameof(localUserService));
//        }

//        [HttpGet]
//        public IActionResult GetUsers()
//        {
//            var users = localUserService.GetUsers();
//            return Ok(users);
//        }
        
//    }
//}
