using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spots.Domain;
using Spots.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.APIs.Controllers
{
    [ApiController]
    [Route("api/vendor/{vendorId}/[controller]")]
    public class FollowController : ControllerBase
    {
        private readonly ISpotsRepositroy repositroy;

        public FollowController(ISpotsRepositroy repositroy)
        {
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
        }
        

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Follow(Guid vendorId, [FromQuery] Guid userId)
        {
            var followed = repositroy.VendorIsFollowedByUser(vendorId, userId);
            if (followed == null)
            {
                var follow = new Follow(vendorId, userId);
                repositroy.AddFollow(follow);
                repositroy.Save();
                var response = new ResponseModel(StatusCodes.Status200OK, "Vendor Followed Successfully",new object());
                return Ok(response);
            }
            else
            {
                repositroy.DeleteFollow(followed);
                repositroy.Save();
                var response = new ResponseModel(StatusCodes.Status200OK, "Vendor Unfollowed Successfully", new object());
                return Ok(response);
            }
        }

    }
}
