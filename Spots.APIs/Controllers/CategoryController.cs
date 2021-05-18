﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spots.Services.ResourceParameters;
using Spots.Domain;
using Spots.DTO;
using Spots.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spots.Services.Helpers;
using System.Text.Json;

namespace Spots.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ISpotsRepositroy repositroy;
        private readonly IWebHostEnvironment hostEnvironment;

        public CategoryController(IMapper mapper, ISpotsRepositroy repositroy, IWebHostEnvironment hostEnvironment)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
            this.hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetCategories")]
        public IActionResult GetCategories([FromQuery] IndexResourceParameters categoryParameters)
        {
            var categories = repositroy.GetCategories(categoryParameters);

            var previousPageLink = categories.HasPrevious ?
                CreateCategoriesResourceUri(categoryParameters, ResourceUriType.PreviousPage) : null;

            var nextPageLink = categories.HasNext ?
                CreateCategoriesResourceUri(categoryParameters, ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = categories.TotalCount,
                pageSize = categories.PageSize,
                currentPage = categories.CurrentPage,
                totalPages = categories.TotalPages,
                previousPageLink,
                nextPageLink
            };

            var paginationForMVC = new
            {
                searchQuery = categoryParameters.SearchQuery,
                currentPage = categories.CurrentPage,
                totalPages = categories.TotalPages,
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
                Data = categories.ToArray()
            });
        }

        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetCategory")]
        public IActionResult GetCategoryById(Guid id)
        {
            if (repositroy.CategoryExists(id))
            {
                var category = repositroy.GetCategoryById(id);
                return Ok(mapper.Map<CategoryDto>(category));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult CreateCategory([FromBody] CategoryForCreationDto category)
        {
            var response = new ResponseModel();

            if (!ModelState.IsValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid Category Format";
                return BadRequest(response);
            }

            var _category = repositroy.GetCategoryByNameAndSuperCategory(category.Name, category.SuperCategoryId);

            if (_category != null)
            {
                response.StatusCode = StatusCodes.Status409Conflict;
                response.Message = $"Category: {category.Name} already exists";
                return Conflict(response);
            }


            _category = mapper.Map<Category>(category);
            if (!repositroy.CategoryExists(category.SuperCategoryId))
            {
                _category.SuperCategoryId = new Guid();
            }

            if(category.Bytes != null)
            {
                // get this environment's web root path (the path
                // from which static content, like an image, is served)
                var webRootPath = hostEnvironment.WebRootPath;

                // create the filename
                string fileName = Guid.NewGuid().ToString() + ".jpg";

                // the full file path
                var filePath = Path.Combine($"{webRootPath}/images/{fileName}");

                // write bytes and auto-close stream
                System.IO.File.WriteAllBytes(filePath, category.Bytes);
                // fill out the filename
                _category.FileName = fileName;
            }
            repositroy.AddCategory(_category);
            repositroy.Save();
            var createdCategoryToReturn = mapper.Map<CategoryDto>(_category);
            response.StatusCode = StatusCodes.Status201Created;
            response.Message = $"Category : '{createdCategoryToReturn.Name }' Created Successfully";
            response.Data = createdCategoryToReturn;
            return CreatedAtRoute("GetCategory", new { createdCategoryToReturn.Id }, response);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(Guid id, [FromBody] CategoryForUpdateDto category,
            [FromQuery] bool imageChanged = false)
        {
            var response = new ResponseModel();

            #region checkvalidityofmodel
            if (!ModelState.IsValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid Category Format";
                return BadRequest(response);
            }
            #endregion
            #region checkexistanceofcategory
            if (!repositroy.CategoryExists(id))
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = ($"Category Not Found");
                return NotFound(response);
            }
            #endregion

            //redefine this condition
            #region checkdublication
            var _category = repositroy.GetCategoryByNameAndSuperCategory(category.Name, category.SuperCategoryId);
            if (_category != null && _category.Id != id)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = $"Category: {category.Name} already exists";
                return BadRequest(response);
            }
            #endregion

            _category = repositroy.GetCategoryById(id);
            mapper.Map(category, _category);

            #region checkexistanceofsupercategory
            if (repositroy.CategoryExists(category.SuperCategoryId))
            {
                if(category.SuperCategoryId == id)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = $"SuperCategory cannot be the exact same category as child";
                    return BadRequest(response);
                }
                _category.SuperCategoryId = category.SuperCategoryId;
            }
            else
            {
                _category.SuperCategoryId = Guid.Empty;
            }
            #endregion

            #region checkimagechange
            if (imageChanged && category.Bytes != null)
            {
                // get this environment's web root path (the path
                // from which static content, like an image, is served)
                var webRootPath = hostEnvironment.WebRootPath;

                var oldImagePath = Path.Combine($"{webRootPath}/images/{_category.FileName}");
                if ((System.IO.File.Exists(oldImagePath)))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                
                // create the filename
                string fileName = Guid.NewGuid().ToString() + ".jpg";

                // the full file path
                var filePath = Path.Combine($"{webRootPath}/images/{fileName}");

                // write bytes and auto-close stream
                System.IO.File.WriteAllBytes(filePath, category.Bytes);

                // fill out the filename
                _category.FileName = fileName;
            }
            #endregion

            repositroy.UpdateCategory(id, _category);
            repositroy.Save();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(Guid id)
        {
            var response = new ResponseModel();
            var category = repositroy.GetCategoryById(id);
            if (category == null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Category to be deleted not found !";
                return NotFound(response);
            }
            if (repositroy.IsSuperCategory(id))
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Category has childred";
                return BadRequest(response);
            }
            var webRootPath = hostEnvironment.WebRootPath;

            var imagePath = Path.Combine($"{webRootPath}/images/{category.FileName}");
            if ((System.IO.File.Exists(imagePath)))
            {
                System.IO.File.Delete(imagePath);
            }
            repositroy.DeleteCategory(category);
            repositroy.Save();
            return NoContent();
        }

        private string CreateCategoriesResourceUri(IndexResourceParameters categoryParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCategories", new
                    {
                        pageNumber = categoryParameters.PageNumber-1,
                        pageSize = categoryParameters.PageSize,
                        seachQuery = categoryParameters.SearchQuery
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCategories", new
                    {
                        pageNumber = categoryParameters.PageNumber + 1,
                        pageSize = categoryParameters.PageSize,
                        seachQuery = categoryParameters.SearchQuery
                    });
                default:
                    return Url.Link("GetCategories", new
                    {
                        pageNumber = categoryParameters.PageNumber,
                        pageSize = categoryParameters.PageSize,
                        seachQuery = categoryParameters.SearchQuery
                    });
            }
        }

    }
}