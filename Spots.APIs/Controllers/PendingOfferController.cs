using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spots.DTO;
using Spots.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.APIs.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("/api/[controller]")]
    public class PendingOfferController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ISpotsRepositroy repositroy;

        public PendingOfferController(IMapper mapper, ISpotsRepositroy repositroy)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
        }

        [HttpGet]
        public IActionResult GetPendingOffers()
        {
            var response = new ResponseModel();
            var offers = repositroy.GetPendingOffers().ToList();
            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "";
            response.Data = mapper.Map<List<OfferDto>>(offers);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public IActionResult AcceptOffer(Guid id)
        {
            var response = new ResponseModel();
            if (!repositroy.OfferExists(id))
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Offer not found";
                return NotFound(response);
            }
            var offer = repositroy.GetSingleOfferById(id);
            offer.OfferApproved = true;
            repositroy.Save();
            return NoContent();
        }

    }
}
