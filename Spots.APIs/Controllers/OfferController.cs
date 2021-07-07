using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Spots.Domain;
using Spots.DTO;
using Spots.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.APIs.Controllers
{
    [ApiController]
    [Route("vendor/{vendorId}/[controller]")]
    [Produces("application/json")]
    [Authorize(Roles = "Admin ,Vendor")]
    public class OfferController : ControllerBase
    {
        private readonly ISpotsRepositroy repositroy;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment hostEnvironment;

        public OfferController(ISpotsRepositroy repositroy, IMapper mapper, IWebHostEnvironment hostEnvironment)
        {
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Offer>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OfferDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public IActionResult GetOffer(Guid vendorId, Guid id, 
                                       [FromQuery] bool includeOfferUses = false)
        {
            var response = new ResponseModel();
            if (!repositroy.VendorExists(vendorId))
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Vendor Doesn't Exist";
                response.Data = new { };
                return NotFound(response);
            }
            var offer = repositroy.GetOfferById(vendorId, id, includeOfferUses);
            var offers = new List<Offer>();
            offers.Add(offer);
            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "";
            response.Data = mapper.Map<IEnumerable<OfferDto>>(offers);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddOffer(Guid vendorId, [FromBody] OfferForCreationDto offer)
        {
            if (!repositroy.VendorExists(vendorId))
            {
                return NotFound($"Vendor {vendorId} does not exist");
            }
            var _offer = mapper.Map<Offer>(offer);
            if (offer.Bytes != null)
            {
                // get this environment's web root path (the path
                // from which static content, like an image, is served)
                var webRootPath = hostEnvironment.WebRootPath;

                // create the filename
                string fileName = Guid.NewGuid().ToString() + ".jpg";

                // the full file path
                var filePath = Path.Combine($"{webRootPath}/images/{fileName}");

                // write bytes and auto-close stream
                System.IO.File.WriteAllBytes(filePath, offer.Bytes);
                // fill out the filename
                _offer.FileName = fileName;
            }
            repositroy.AddOffer(vendorId,_offer);
            repositroy.Save();
            if (_offer.OfferApproved)
            {
                //var pushController = new TopicNotificationController(hostEnvironment);
                //var pushNotificationResult = await pushController.PushTopicNotification();
                var vendor = repositroy.GetVendorById(vendorId, false);
                var note = new FcmTopicNotification(hostEnvironment);
                await note.OnGetAsync($"New Offer From {vendor.Name}"
                    ,_offer.Description,_offer.VendorId.ToString());
            }

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
