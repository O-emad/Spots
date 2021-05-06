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
        public IActionResult GetReviews(Guid vendorId)
        {
            if (!repositroy.VendorExists(vendorId))
            {
                return NotFound();
            }
            var reviews = repositroy.GetReviewsForVendor(vendorId);
            return Ok(mapper.Map<IEnumerable<ReviewDto>>(reviews));
        }
        [HttpGet("{id}", Name = "GetReview")]
        public IActionResult GetReview(Guid vendorId, Guid id)
        {
            if (!repositroy.VendorExists(vendorId))
            {
                return NotFound();
            }
            var review = repositroy.GetReviewById(vendorId, id);
            return Ok(mapper.Map<ReviewDto>(review));
        }

        [HttpPost]
        public IActionResult AddReview(Guid vendorId, [FromBody] ReviewForCreationDto review)
        {
            if (!repositroy.VendorExists(vendorId))
            {
                return NotFound($"Vendor {vendorId} does not exist");
            }
            var _review = mapper.Map<Review>(review);
            repositroy.AddReview(vendorId, _review);
            repositroy.Save();

            var createdReviewToReturn = mapper.Map<ReviewDto>(_review);
            return CreatedAtRoute("GetReview", new { createdReviewToReturn.VendorId, createdReviewToReturn.Id },
                createdReviewToReturn);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateReview(Guid vendorId, Guid id, [FromBody] ReviewForUpdateDto review)
        {
            if (!repositroy.VendorExists(vendorId))
            {
                return NotFound($"Vendor {vendorId} does not exist");
            }
            var _review = repositroy.GetReviewById(vendorId, id);
            if(_review == null)
            {
                return NotFound();
            }
            mapper.Map(review, _review);
            repositroy.UpdateReview(vendorId, id, _review);
            repositroy.Save();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReview(Guid vendorId, Guid id)
        {
            if (!repositroy.VendorExists(vendorId))
            {
                return NotFound($"Vendor {vendorId} does not exist");
            }
            var review = repositroy.GetReviewById(vendorId, id);
            if (review == null)
            {
                return NotFound();
            }
            repositroy.DeleteReview(review);
            repositroy.Save();
            return NoContent();
        }
    }
}
