using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spots.Domain;
using Spots.DTO;
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
    [Authorize(Roles = "Admin ,Vendor")]
    public class OfferController : ControllerBase
    {
        private readonly ISpotsRepositroy repositroy;
        private readonly IMapper mapper;

        public OfferController(ISpotsRepositroy repositroy, IMapper mapper)
        {
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetOffers(Guid vendorId)
        {
            var response = new ResponseModel();
            if (!repositroy.VendorExists(vendorId))
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Vendor doesn't exist";
                response.Data = new { };
                return NotFound(response);
            }
            var offers = repositroy.GetOffersForVendor(vendorId).ToList();
            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "";
            response.Data = mapper.Map<List<OfferDto>>(offers);
            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetOffer")]
        [AllowAnonymous]
        public IActionResult GetOffer(Guid vendorId, Guid id)
        {
            var response = new ResponseModel();
            if (!repositroy.VendorExists(vendorId))
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Vendor Doesn't Exist";
                response.Data = new { };
                return NotFound(response);
            }
            var offer = repositroy.GetOfferById(vendorId, id);
            var offers = new List<Offer>();
            offers.Add(offer);
            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "";
            response.Data = mapper.Map<IEnumerable<OfferDto>>(offers);
            return Ok(response);
        }

        [HttpPost]
        public IActionResult AddOffer(Guid vendorId, [FromBody] OfferForCreationDto offer)
        {
            if (!repositroy.VendorExists(vendorId))
            {
                return NotFound($"Vendor {vendorId} does not exist");
            }
            var _offer = mapper.Map<Offer>(offer);
            repositroy.AddOffer(vendorId,_offer);
            repositroy.Save();

            var createdOfferToReturn = mapper.Map<OfferDto>(_offer);
            return CreatedAtRoute("GetOffer", new { createdOfferToReturn.VendorId, createdOfferToReturn.Id },
                createdOfferToReturn);
        }

        //offers cannot be updated
        //public IActionResult UpdateOffer(Guid vendorId, Guid id, [FromBody] OfferForUpdateDto offer)
        //{
        //    if (!repositroy.VendorExists(vendorId))
        //    {
        //        return NotFound($"Vendor {vendorId} does not exist");
        //    }

        //    var _offer = repositroy.GetOfferById(vendorId, id);
        //    mapper.Map(offer, _offer);

        //}

        [HttpDelete("{id}")]
        public IActionResult DeleteOffer(Guid vendorId, Guid id)
        {
            var response = new ResponseModel();
            if (!repositroy.VendorExists(vendorId))
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = $"Vendor {vendorId} doesn't exist";
                response.Data = new { };
                return NotFound(response);
            }
            var offer = repositroy.GetOfferById(vendorId, id);
            if(offer == null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = $"Offer doesn't exist";
                response.Data = new { };
                return NotFound(response);
            }
            repositroy.DeleteOffer(offer);
            repositroy.Save();
            return NoContent();
        }

    }
}
