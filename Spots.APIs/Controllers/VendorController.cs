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
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly ISpotsRepositroy repositroy;
        private readonly IMapper mapper;

        public VendorController(ISpotsRepositroy repositroy, IMapper mapper)
        {
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult GetVendors()
        {
            var vendors = repositroy.GetVendors();
            return Ok(mapper.Map<IEnumerable<VendorDto>>(vendors));
        }

        [HttpGet("{id}", Name ="GetVendor")]
        public IActionResult GetVendorById(Guid id)
        {
            if (repositroy.VendorExists(id))
            {
                var vendor = repositroy.GetVendorById(id);
                return Ok(mapper.Map<VendorDto>(vendor));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult CreateVendor([FromBody]VendorForCreationDto vendor)
        {
            var _vendor = repositroy.GetVendorByName(vendor.Name);
            if(_vendor != null)
            {
                return BadRequest($"Vendor: {vendor.Name} already exists");
            }
            _vendor = mapper.Map<Vendor>(vendor);
            repositroy.AddVendor(_vendor);
            repositroy.Save();

            var CreatedVendorToReturn = mapper.Map<VendorDto>(_vendor);
            return CreatedAtRoute("GetVendor", new { CreatedVendorToReturn.Id }, CreatedVendorToReturn);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateVendor(Guid id, [FromBody]VendorForUpdateDto vendor)
        {
            if (!repositroy.VendorExists(id))
            {
                return NotFound();
            }
            var _vendor = repositroy.GetVendorByName(vendor.Name);
            if (_vendor != null)
            {
                return BadRequest($"Vendor: {vendor.Name} already exists");
            }
            _vendor = repositroy.GetVendorById(id);
            mapper.Map(vendor, _vendor);
            repositroy.UpdateVendor(id, _vendor);
            repositroy.Save();
            return NoContent();

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteVendor(Guid id)
        {
            var vendor = repositroy.GetVendorById(id);
            if (vendor == null)
            {
                return NotFound();
            }
            repositroy.DeleteVendor(vendor);
            repositroy.Save();
            return NoContent();
        }


    }
}
