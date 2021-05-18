using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spots.Domain;
using Spots.DTO;
using Spots.Services;
using Spots.Services.Helpers;
using Spots.Services.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Spots.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly ISpotsRepositroy repositroy;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment hostEnvironment;

        public VendorController(ISpotsRepositroy repositroy, IMapper mapper, IWebHostEnvironment hostEnvironment)
        {
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetVendors")]
        public IActionResult GetVendors([FromQuery] IndexResourceParameters vendorParameters)
        {
            var vendors = repositroy.GetVendors(vendorParameters);
            var previousPageLink = vendors.HasPrevious ?
                CreateVendorsResourceUri(vendorParameters, ResourceUriType.PreviousPage) : null;

            var nextPageLink = vendors.HasNext ?
                CreateVendorsResourceUri(vendorParameters, ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = vendors.TotalCount,
                pageSize = vendors.PageSize,
                currentPage = vendors.CurrentPage,
                totalPages = vendors.TotalPages,
                previousPageLink,
                nextPageLink
            };

            var paginationForMVC = new
            {
                searchQuery = vendorParameters.SearchQuery,
                currentPage = vendors.CurrentPage,
                totalPages = vendors.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));
            Response.Headers.Add("MVC-Pagination",
                JsonSerializer.Serialize(paginationForMVC));

            return Ok(new ResponseModel()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "",
                Data = mapper.Map<IEnumerable<VendorDto>>(vendors).ToList()
            });
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

        private string CreateVendorsResourceUri(IndexResourceParameters vendorsParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCategories", new
                    {
                        pageNumber = vendorsParameters.PageNumber - 1,
                        pageSize = vendorsParameters.PageSize,
                        seachQuery = vendorsParameters.SearchQuery
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCategories", new
                    {
                        pageNumber = vendorsParameters.PageNumber + 1,
                        pageSize = vendorsParameters.PageSize,
                        seachQuery = vendorsParameters.SearchQuery
                    });
                default:
                    return Url.Link("GetCategories", new
                    {
                        pageNumber = vendorsParameters.PageNumber,
                        pageSize = vendorsParameters.PageSize,
                        seachQuery = vendorsParameters.SearchQuery
                    });
            }
        }
    }
}
