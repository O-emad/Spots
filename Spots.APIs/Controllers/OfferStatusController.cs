using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("[controller]")]
    public class OfferStatusController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ISpotsRepositroy repositroy;

        public OfferStatusController(IMapper mapper, ISpotsRepositroy repositroy)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
        }


        [HttpPut("{id}")]
        public IActionResult ChangeOfferStatus(Guid id)
        {
            var response = new ResponseModel();
            if (!repositroy.OfferExists(id))
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Offer not found";
                return NotFound(response);
            }
            var offer = repositroy.GetSingleOfferById(id);
            offer.Enabled = !offer.Enabled; 
            repositroy.Save();
            return NoContent();
        }

    }

}
