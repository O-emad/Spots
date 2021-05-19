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
using System.IO;
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

        [HttpGet("{id}", Name = "GetVendor")]
        public IActionResult GetVendorById(Guid id)
        {
            if (repositroy.VendorExists(id))
            {
                var vendor = repositroy.GetVendorById(id);
                var vendors = new List<Vendor>();
                vendors.Add(vendor);
                return Ok(new ResponseModel()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "",
                    Data = mapper.Map<IEnumerable<VendorDto>>(vendors).ToList()
                });;
            }
            else
            {
                return NotFound(new ResponseModel()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Vendor Not Found",
                    Data = new { }
                });
            }
        }

        [HttpPost]
        public IActionResult CreateVendor([FromBody] VendorForCreationDto vendor)
        {
            var response = new ResponseModel();

            if (!ModelState.IsValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid Vendor Format";
                return BadRequest(response);
            }

            var _vendor = repositroy.GetVendorByName(vendor.Name);

            if (_vendor != null)
            {
                response.StatusCode = StatusCodes.Status409Conflict;
                response.Message = $"Vendor: {vendor.Name} already exists";
                return Conflict(response);
            }


            _vendor = mapper.Map<Vendor>(vendor);

            if (vendor.ProfileBytes != null)
            {
                // get this environment's web root path (the path
                // from which static content, like an image, is served)
                var webRootPath = hostEnvironment.WebRootPath;

                // create the filename
                string fileName = Guid.NewGuid().ToString() + ".jpg";

                // the full file path
                var filePath = Path.Combine($"{webRootPath}/images/{fileName}");

                // write bytes and auto-close stream
                System.IO.File.WriteAllBytes(filePath, vendor.ProfileBytes);
                // fill out the filename
                _vendor.ProfilePicFileName = fileName;
            }
            if (vendor.BannerBytes != null)
            {
                // get this environment's web root path (the path
                // from which static content, like an image, is served)
                var webRootPath = hostEnvironment.WebRootPath;

                // create the filename
                string fileName = Guid.NewGuid().ToString() + ".jpg";

                // the full file path
                var filePath = Path.Combine($"{webRootPath}/images/{fileName}");

                // write bytes and auto-close stream
                System.IO.File.WriteAllBytes(filePath, vendor.BannerBytes);
                // fill out the filename
                _vendor.BannerPicFileName = fileName;
            }
            repositroy.AddVendor(_vendor);
            repositroy.Save();
            var createdVendorToReturn = mapper.Map<VendorDto>(_vendor);
            response.StatusCode = StatusCodes.Status201Created;
            response.Message = $"Vendor : '{createdVendorToReturn.Name }' Created Successfully";
            response.Data = createdVendorToReturn;
            return CreatedAtRoute("GetVendor", new { createdVendorToReturn.Id }, response);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateVendor(Guid id, [FromBody] VendorForUpdateDto vendor, 
            [FromQuery] bool imageChanged = false)
        {

            var response = new ResponseModel();

            #region checkvalidityofmodel
            if (!ModelState.IsValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid Vendor Format";
                return BadRequest(response);
            }
            #endregion
            #region checkexistanceofcategory
            if (!repositroy.VendorExists(id))
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = ($"Vendor Not Found");
                return NotFound(response);
            }
            #endregion
            #region checkdublication
            var _vendor = repositroy.GetVendorByName(vendor.Name);
            if (_vendor != null && _vendor.Id != id)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = $"Vendor: {vendor.Name} already exists";
                return BadRequest(response);
            }
            #endregion

            _vendor = repositroy.GetVendorById(id);
            //.Map(vendor, _vendor);    
            {
                _vendor.Name = vendor.Name;
                _vendor.Location = vendor.Location;
                _vendor.OpenAt = vendor.OpenAt;
                _vendor.CloseAt = vendor.CloseAt;
                _vendor.SortOrder = vendor.SortOrder;

            }
            #region checkimagechange
            if (imageChanged && vendor.ProfileBytes != null)
            {
                // get this environment's web root path (the path
                // from which static content, like an image, is served)
                var webRootPath = hostEnvironment.WebRootPath;

                var oldImagePath = Path.Combine($"{webRootPath}/images/{_vendor.ProfilePicFileName}");
                if ((System.IO.File.Exists(oldImagePath)))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // create the filename
                string fileName = Guid.NewGuid().ToString() + ".jpg";

                // the full file path
                var filePath = Path.Combine($"{webRootPath}/images/{fileName}");

                // write bytes and auto-close stream
                System.IO.File.WriteAllBytes(filePath, vendor.ProfileBytes);

                // fill out the filename
                _vendor.ProfilePicFileName = fileName;
            }

            if (imageChanged && vendor.BannerBytes != null)
            {
                // get this environment's web root path (the path
                // from which static content, like an image, is served)
                var webRootPath = hostEnvironment.WebRootPath;

                var oldImagePath = Path.Combine($"{webRootPath}/images/{_vendor.BannerPicFileName}");
                if ((System.IO.File.Exists(oldImagePath)))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // create the filename
                string fileName = Guid.NewGuid().ToString() + ".jpg";

                // the full file path
                var filePath = Path.Combine($"{webRootPath}/images/{fileName}");

                // write bytes and auto-close stream
                System.IO.File.WriteAllBytes(filePath, vendor.BannerBytes);

                // fill out the filename
                _vendor.BannerPicFileName = fileName;
            }
            #endregion

            repositroy.UpdateVendor(id, _vendor, vendor.Categories);
            repositroy.Save();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteVendor(Guid id)
        {
            var response = new ResponseModel();
            var vendor = repositroy.GetVendorById(id);
            if (vendor == null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Vendor to be deleted not found !";
                return NotFound(response);
            }
            var webRootPath = hostEnvironment.WebRootPath;

            var imagePath = Path.Combine($"{webRootPath}/images/{vendor.ProfilePicFileName}");
            if ((System.IO.File.Exists(imagePath)))
            {
                System.IO.File.Delete(imagePath);
            }
            imagePath = Path.Combine($"{webRootPath}/images/{vendor.BannerPicFileName}");
            if ((System.IO.File.Exists(imagePath)))
            {
                System.IO.File.Delete(imagePath);
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
