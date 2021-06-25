using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
    [Route("vendor/{vendorId}/[controller]")]
    [Produces("application/json")]
    public class VendorGalleryController : ControllerBase
    {
        private readonly ISpotsRepositroy repositroy;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment hostEnvironment;

        public VendorGalleryController(ISpotsRepositroy repositroy, IMapper mapper, IWebHostEnvironment hostEnvironment)
        {
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<VendorGalleryDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetGallerys(Guid vendorId)
        {
            var response = new ResponseModel();
            if (!repositroy.VendorExists(vendorId))
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Vendor doesn't exist";
                return NotFound(response);
            }
            var galleries = repositroy.GetGalleriesForVendor(vendorId);
            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "";
            response.Data = new { gallery = mapper.Map<List<VendorGalleryDto>>(galleries) };
            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetGallery")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VendorGalleryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetGallery(Guid vendorId, Guid id)
        {
            var response = new ResponseModel();
            response.StatusCode = StatusCodes.Status404NotFound;
            if (!repositroy.VendorExists(vendorId))
            {
                response.Message = "Vendor doesn't exist";
                return NotFound(response);
            }
            var gallery = repositroy.GetGalleryById(id);
            if (gallery == null)
            {
                response.Message = "Gallery doesn't exist";
                return NotFound(response);
            }
            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "";
            response.Data = new { gallery = mapper.Map<VendorGalleryDto>(gallery) };
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VendorGalleryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AddGallery(Guid vendorId, [FromBody] VendorGalleryForCreationDto vendorGallery)
        {

            if (!repositroy.VendorExists(vendorId))
            {
                var response = new ResponseModel();
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Vendor doesn't exist";
                return NotFound(response);
            }
            var gallery = mapper.Map<VendorGallery>(vendorGallery);
            var mediaManager = new MediaManager(hostEnvironment);
            gallery.FileName = mediaManager.UploadPhoto(vendorGallery.FileBytes);
            repositroy.AddGallery(vendorId, gallery);
            repositroy.Save();
            var createdGalleryToReturn = mapper.Map<VendorGalleryDto>(gallery);
            return CreatedAtRoute("GetGallery", new { createdGalleryToReturn.VendorId, createdGalleryToReturn.Id }
                                  , createdGalleryToReturn);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteGallery(Guid vendorId, Guid id)
        {
            var response = new ResponseModel();
            response.StatusCode = StatusCodes.Status404NotFound;
            if (!repositroy.VendorExists(vendorId))
            {
                response.Message = "Vendor doesn't exist";
                return NotFound(response);
            }
            var gallery = repositroy.GetGalleryById(id);
            if (gallery == null)
            {
                response.Message = "Gallery doesn't exist";
                return NotFound(response);
            }
            var mediaManager = new MediaManager(hostEnvironment);
            mediaManager.DeletePhoto(gallery.FileName);
            repositroy.DeleteGallery(gallery);
            repositroy.Save();
            return NoContent();

        }

    }

}
