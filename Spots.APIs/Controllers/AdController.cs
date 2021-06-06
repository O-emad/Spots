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
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class AdController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ISpotsRepositroy repositroy;
        private readonly IWebHostEnvironment hostEnvironment;

        public AdController(IMapper mapper, ISpotsRepositroy repositroy, IWebHostEnvironment hostEnvironment)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
            this.hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        [HttpGet(Name = "GetAds")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Ad>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public IActionResult GetAds([FromQuery] IndexResourceParameters adParameters)
        {
            var ads = repositroy.GetAds(adParameters);
            var previousPageLink = ads.HasPrevious ?
              CreateAdsResourceUri(adParameters, ResourceUriType.PreviousPage) : null;

            var nextPageLink = ads.HasNext ?
                CreateAdsResourceUri(adParameters, ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = ads.TotalCount,
                pageSize = ads.PageSize,
                currentPage = ads.CurrentPage,
                totalPages = ads.TotalPages,
                previousPageLink,
                nextPageLink
            };

            var paginationForMVC = new
            {
                searchQuery = adParameters.SearchQuery,
                currentPage = ads.CurrentPage,
                totalPages = ads.TotalPages,
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
                Data = ads.ToList()
            });
        }


        [HttpGet("{id}", Name = "GetAd")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public IActionResult GetAd(Guid id)
        {
            if (repositroy.AdExists(id))
            {
                var ad = repositroy.GetAdById(id);
                var ads = new List<Ad>();
                ads.Add(ad);
                return Ok(new ResponseModel()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "",
                    Data = mapper.Map<IEnumerable<AdDto>>(ads).ToList()
                }); ;
            }
            else
            {
                return NotFound(new ResponseModel()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Ad Not Found",
                    Data = new { }
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateAd([FromBody]AdForCreationDto ad)
        {
            var response = new ResponseModel();

            if (!ModelState.IsValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid Ad Format";
                return BadRequest(response);
            }


            //check conflict state if required
            //var _category = repositroy.GetCategoryByNameAndSuperCategory(category.Name, category.CategoryId);

            //if (_category != null)
            //{
            //    response.StatusCode = StatusCodes.Status409Conflict;
            //    response.Message = $"Category: {category.Name} already exists";
            //    return Conflict(response);
            //}

            var _ad = mapper.Map<Ad>(ad);

            if (ad.File != null)
            {
                // get this environment's web root path (the path
                // from which static content, like an image, is served)
                var webRootPath = hostEnvironment.WebRootPath;

                // create the filename
                string fileName = Guid.NewGuid().ToString() + ".jpg";

                // the full file path
                var filePath = Path.Combine($"{webRootPath}/images/{fileName}");

                // write bytes and auto-close stream
                System.IO.File.WriteAllBytes(filePath, ad.File);
                // fill out the filename
                _ad.FileName = fileName;
            }

            repositroy.AddAd(_ad);
            repositroy.Save();
            var createdAdToReturn = mapper.Map<AdDto>(_ad);
            response.StatusCode = StatusCodes.Status201Created;
            response.Message = $"Ad : '{createdAdToReturn.Name }' Created Successfully";
            response.Data = createdAdToReturn;
            return CreatedAtRoute("GetAd", new { createdAdToReturn.Id }, response);
        }
        private string CreateAdsResourceUri(IndexResourceParameters adParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCategories", new
                    {
                        pageNumber = adParameters.PageNumber - 1,
                        pageSize = adParameters.PageSize,
                        seachQuery = adParameters.SearchQuery
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCategories", new
                    {
                        pageNumber = adParameters.PageNumber + 1,
                        pageSize = adParameters.PageSize,
                        seachQuery = adParameters.SearchQuery
                    });
                default:
                    return Url.Link("GetCategories", new
                    {
                        pageNumber = adParameters.PageNumber,
                        pageSize = adParameters.PageSize,
                        seachQuery = adParameters.SearchQuery
                    });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteAd(Guid id)
        {
            var response = new ResponseModel();
            var ad = repositroy.GetAdById(id);
            if (ad == null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Ad to be deleted not found !";
                return NotFound(response);
            }
            var webRootPath = hostEnvironment.WebRootPath;

            var imagePath = Path.Combine($"{webRootPath}/images/{ad.FileName}");
            if ((System.IO.File.Exists(imagePath)))
            {
                System.IO.File.Delete(imagePath);
            }
            repositroy.DeleteAd(ad);
            repositroy.Save();
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateAd(Guid id, [FromBody] AdForUpdateDto ad,
            [FromQuery] bool imageChanged = false)
        {

            var response = new ResponseModel();

            #region checkvalidityofmodel
            if (!ModelState.IsValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid Ad Format";
                return BadRequest(response);
            }
            #endregion
            #region checkexistanceofcategory
            if (!repositroy.AdExists(id))
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = ($"Ad Not Found");
                return NotFound(response);
            }
            #endregion
            #region checkdublication

            #endregion

            var _ad = repositroy.GetAdById(id);
            //.Map(vendor, _vendor);    
            mapper.Map(ad, _ad);
            #region checkimagechange
            if (imageChanged && ad.File != null)
            {
                // get this environment's web root path (the path
                // from which static content, like an image, is served)
                var webRootPath = hostEnvironment.WebRootPath;

                var oldImagePath = Path.Combine($"{webRootPath}/images/{_ad.FileName}");
                if ((System.IO.File.Exists(oldImagePath)))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // create the filename
                string fileName = Guid.NewGuid().ToString() + ".jpg";

                // the full file path
                var filePath = Path.Combine($"{webRootPath}/images/{fileName}");

                // write bytes and auto-close stream
                System.IO.File.WriteAllBytes(filePath, ad.File);

                // fill out the filename
                _ad.FileName = fileName;
            }
            #endregion

            repositroy.UpdateAd(id, _ad);
            repositroy.Save();
            return NoContent();
        }

    }
}
