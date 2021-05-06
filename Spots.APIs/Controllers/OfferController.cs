using AutoMapper;
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
        public IActionResult GetOffers(Guid vendorId)
        {
            var offers = repositroy.GetOffersForVendor(vendorId);
            return Ok(mapper.Map<IEnumerable<OfferDto>>(offers));
        }

        [HttpGet("{id}", Name = "GetOffer")]
        public IActionResult GetOffer(Guid vendorId, Guid id)
        {
            var offer = repositroy.GetOfferById(vendorId, id);
            return Ok(mapper.Map<OfferDto>(offer));
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

        public IActionResult DeleteOffer(Guid vendorId, Guid id)
        {
            if (!repositroy.VendorExists(vendorId))
            {
                return NotFound($"Vendor {vendorId} does not exist");
            }
            var offer = repositroy.GetOfferById(vendorId, id);
            if(offer == null)
            {
                return NotFound();
            }
            repositroy.DeleteOffer(offer);
            repositroy.Save();
            return NoContent();
        }

    }
}
