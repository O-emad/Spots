using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spots.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OfferQrScanController : ControllerBase
    {
        private readonly ISpotsRepositroy repositroy;

        public OfferQrScanController(ISpotsRepositroy repositroy)
        {
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
        }



        [HttpPost("{offerId}")]
        public IActionResult UseQrCode(Guid offerId, string vendorName)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var offer = repositroy.GetSingleOfferById(offerId);
            if(offer == null)
            {
                return NotFound(new ResponseModel()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Offer not found"
                });
            }
            var vendor = repositroy.GetVendorById(offer.VendorId, false);
            if(vendor.Name != vendorName)
            {
                return BadRequest(new ResponseModel()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = $"Vendor: {vendorName} doesn't have the following offer"
                });
            }
            //send a push notification 
            return NoContent();
        }

    }
}
