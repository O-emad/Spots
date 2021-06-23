using AutoMapper;
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
    [Produces("application/json")]
    public class VendorVideoController : ControllerBase
    {
        private readonly ISpotsRepositroy repositroy;
        private readonly IMapper mapper;

        public VendorVideoController(ISpotsRepositroy repositroy, IMapper mapper)
        {
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type =typeof(List<VendorVideoDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetVideos(Guid vendorId)
        {
            var response = new ResponseModel();
            if (!repositroy.VendorExists(vendorId))
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Vendor doesn't exist";
                return NotFound(response);
            }
            var videos = repositroy.GetVideosForVendor(vendorId);
            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "";
            response.Data = new { video = mapper.Map<List<VendorVideoDto>>(videos) };
            return Ok(response);
        }

        [HttpGet("{id}",Name = "GetVideo")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VendorVideoDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetVideo(Guid vendorId, Guid id)
        {
            var response = new ResponseModel();
            response.StatusCode = StatusCodes.Status404NotFound;
            if (!repositroy.VendorExists(vendorId)){
                response.Message = "Vendor doesn't exist";
                return NotFound(response);
            }
            var video = repositroy.GetVideoById(id);
            if (video == null)
            {
                response.Message = "Video doesn't exist";
                return NotFound(response);
            }
            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "";
            response.Data = new { video = mapper.Map<VendorVideoDto>(video) };
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VendorVideoDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AddVideo(Guid vendorId, [FromBody]VendorVideoForCreationDto vendorVideo)
        {
            
            if (!repositroy.VendorExists(vendorId))
            {
                var response = new ResponseModel();
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Vendor doesn't exist";
                return NotFound(response);
            }
            var video = mapper.Map<VendorVideo>(vendorVideo);
            repositroy.AddVideo(vendorId, video);
            repositroy.Save();
            var createdVideoToReturn = mapper.Map<VendorVideoDto>(video);
            return CreatedAtRoute("GetVideo", new { createdVideoToReturn.VendorId, createdVideoToReturn.Id }
                                  , createdVideoToReturn);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteVideo(Guid vendorId, Guid id)
        {
            var response = new ResponseModel();
            response.StatusCode = StatusCodes.Status404NotFound;
            if (!repositroy.VendorExists(vendorId))
            {
                response.Message = "Vendor doesn't exist";
                return NotFound(response);
            }
            var video = repositroy.GetVideoById(id);
            if(video == null)
            {
                response.Message = "Video doesn't exist";
                return NotFound(response);
            }
            repositroy.DeleteVideo(video);
            repositroy.Save();
            return NoContent();

        }
        
    }
}
