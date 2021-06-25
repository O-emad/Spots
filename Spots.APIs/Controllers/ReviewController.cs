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
    [Produces("application/json")]
    [ApiController]
    [Route("vendor/{vendorId}/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ISpotsRepositroy repositroy;
        private readonly IMapper mapper;

        public ReviewController(ISpotsRepositroy repositroy, IMapper mapper)
        {
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ReviewDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public IActionResult GetReviews(Guid vendorId)
        {
            var response = new ResponseModel();
            if (!repositroy.VendorExists(vendorId))
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Vendor Doesn't exist";
                return NotFound(response);
            }
            var reviews = repositroy.GetReviewsForVendor(vendorId);
            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "";
            response.Data = new { reviews = mapper.Map<IEnumerable<ReviewDto>>(reviews) };
            return Ok(response);
        }
        [HttpGet("{id}", Name = "GetReview")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReviewDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetReview(Guid vendorId, Guid id)
        {
            var response = new ResponseModel();
            response.StatusCode = StatusCodes.Status404NotFound;
            if (!repositroy.VendorExists(vendorId))
            {
                response.Message = "Vendor doesn't exist";
                return NotFound(response);
            }
            var review = repositroy.GetReviewById(vendorId, id);
            if(review == null)
            {
                response.Message = "Review doesn't exist";
                return NotFound(response);
            }
            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "";
            response.Data = new { review = mapper.Map<ReviewDto>(review) };
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReviewDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult AddReview(Guid vendorId, [FromBody] ReviewForCreationDto review)
        {
            
            if (!repositroy.VendorExists(vendorId))
            {
                var response = new ResponseModel();
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Vendor doesn't exist";
                return NotFound(response);
            }
            var _review = mapper.Map<Review>(review);
            repositroy.AddReview(vendorId, _review);
            repositroy.Save();

            var createdReviewToReturn = mapper.Map<ReviewDto>(_review);
            return CreatedAtRoute("GetReview", new { createdReviewToReturn.VendorId, createdReviewToReturn.Id },
                createdReviewToReturn);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateReview(Guid vendorId, Guid id, [FromBody] ReviewForUpdateDto review)
        {
            var response = new ResponseModel();
            response.StatusCode = StatusCodes.Status404NotFound;
            if (!repositroy.VendorExists(vendorId))
            {
                response.Message = "Vendor doesn't exist";
                return NotFound(response);
            }
            var _review = repositroy.GetReviewById(vendorId, id);
            if(_review == null)
            {
                response.Message = "Review doesn't exist";
                return NotFound(response);
            }
            mapper.Map(review, _review);
            repositroy.UpdateReview(vendorId, id, _review);
            repositroy.Save();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteReview(Guid vendorId, Guid id)
        {
            var response = new ResponseModel();
            response.StatusCode = StatusCodes.Status404NotFound;
            if (!repositroy.VendorExists(vendorId))
            {
                response.Message = "Vendor doesn't exist";
                return NotFound(response);
            }
            var review = repositroy.GetReviewById(vendorId, id);
            if (review == null)
            {
                response.Message = "Review doesn't exist";
                return NotFound(response);
            }
            repositroy.DeleteReview(review);
            repositroy.Save();
            return NoContent();
        }
    }
}
