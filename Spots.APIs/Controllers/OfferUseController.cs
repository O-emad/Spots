using AutoMapper;
using Marvin.IDP.Services;
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
    [Route("vendor/{vendorId}/offer/{offerId}/[controller]")]
    [Produces("application/json")]
    public class OfferUseController : ControllerBase
    {
        private readonly ISpotsRepositroy repositroy;
        private readonly IMapper mapper;
        private readonly ILocalUserService localUserService;

        public OfferUseController(ISpotsRepositroy repositroy, IMapper mapper, ILocalUserService localUserService)
        {
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.localUserService = localUserService ?? throw new ArgumentNullException(nameof(localUserService));
        }

        //[HttpGet]
        //public IActionResult GetOfferUses()
        //{

        //}

        [HttpPost]
        public async Task<IActionResult> UseOffer(Guid vendorId, Guid offerId)
        {
            if (!User.IsInRole("User"))
            {
                var response = new ResponseModel()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Offers are only usable by users"
                };
                return BadRequest(response);
            }
            if (!repositroy.VendorExists(vendorId))
            {
                var response = new ResponseModel()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Vendor does not exist"
                };
                return NotFound(response);
            }
            if (!repositroy.OfferExists(offerId))
            {
                var response = new ResponseModel()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Offer does not exist"
                };
                return NotFound(response);
            }
            var userSubject = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrWhiteSpace(userSubject))
            {
                var response = new ResponseModel()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "User does not exist"
                };
                return NotFound(response);
            }
            if (!repositroy.EligibleOfferUse(offerId, userSubject))
            {
                var response = new ResponseModel()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "This user has fully consumed this offer"
                };
                return BadRequest(response);
            }
            var user = await localUserService.GetUserBySubjectAsync(userSubject);
            var fullName = user.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value +
                user.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value;
            var offerUse = new OfferUse()
            {
                OfferId = offerId,
                UserSubject = userSubject,
                UserName = user.UserName,
                Name = fullName
            };
            repositroy.AddOfferUse(offerUse);
            repositroy.Save();
            return Ok(new ResponseModel()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Offer used successfully"
            });
        }
        
    }
}
