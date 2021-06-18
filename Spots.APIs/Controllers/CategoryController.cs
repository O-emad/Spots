using AutoMapper;
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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Spots.Services.Helpers.ResourceParameters;
using Microsoft.Extensions.Primitives;

namespace Spots.APIs.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
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

        /// <summary>
        /// Get all categories
        /// </summary>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CategoryDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet(Name = "GetCategories")]
        public IActionResult GetCategories([FromQuery] CategoryResourceParameters categoryParameters,
                                           [FromHeader(Name = "X-Lang")] string language = "en")
        {
            var categories = repositroy.GetCategories(categoryParameters, language);

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
            var response = new ResponseModel();


            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "";
            response.Data = mapper.Map<List<CategoryDto>>(categories);
 

            return Ok(response);
        }


        /// <summary>
        /// Get a category by its id
        /// </summary>
        /// <param name="id">The id of the category you want to get</param>
        /// <param name="includeSub">Query parameter to include the sub categories</param>
        /// <param name="includeVendors">Query parameter to include the subscriped vendors</param>
        /// <param name="language">Language header to choose return language "only 'en' and 'ar' are available"</param>
        /// <returns>a Category</returns>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}", Name = "GetCategory")]
        public IActionResult GetCategoryById(Guid id, [FromQuery] bool includeVendors = false,
            [FromQuery] bool includeSub = false, [FromHeader(Name ="X-Lang")] string language = "en")
        {
            var response = new ResponseModel();
            //returns a domain category with both language
            if (repositroy.CategoryExists(id))
            {
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "";
                if (language.ToLower().Trim().Contains("all"))
                {
                    response.Data = new List<Category>() { repositroy.GetCategoryById(id, includeVendors, includeSub, language) };
                }
                else
                {
                    var category = repositroy.GetCategoryById(id, includeVendors, includeSub, language);
                    response.Data = mapper.Map<IEnumerable<CategoryDto>>( new List<Category>() {category});
                }
                
                return Ok(response);
            }
            else
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Category not found";
                return NotFound(response);
            }
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public IActionResult CreateCategory([FromBody] CategoryForCreationDto category)
        {
            var response = new ResponseModel();

            #region check existance of parent category
            //check availability of parent category in the model
            try
            {
                var parentCategoryExists = repositroy.CategoryExists(category.CategoryId.Value);
                //if the repo returns null, then the given parent category id is invalid
                if (!parentCategoryExists)
                {
                    ModelState.AddModelError("CategoryId",
                        "The given parent category id doesn't exist");
                }
            }
            catch(Exception e)
            {
                //check if the reason of exception is that the parent category is null
                if(e.GetType() == typeof(InvalidOperationException))
                {
                    //if that's the case then we got nothing to do
                }
                else
                {
                    throw;
                }
            }
            #endregion
            #region check validity of model
            //check model state validity
            if (!ModelState.IsValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid Category Format";
                response.Data = ModelState;
                return BadRequest(response);
            }
            #endregion
            #region check dublication of category
            var _category = repositroy.GetCategoryByNameAndSuperCategory(category.Name, category.NameAR, category.CategoryId);
            //check conflict with pre existing categories
            if (_category != null)
            {
                response.StatusCode = StatusCodes.Status409Conflict;
                response.Message = $"Category: {category.Name} already exists";
                return Conflict(response);
            }
            #endregion

            //map creation dto into an entity
            _category = mapper.Map<Category>(category);
            //save the image if exists
            if (category.Bytes != null)
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
            _category.Names = new List<Name>()
            {
                new Name(){Value = category.Name, Culture = "en"},
                new Name(){Value = category.NameAR, Culture = "ar"}
            };
            repositroy.AddCategory(_category);
            repositroy.Save();
            var createdCategoryToReturn = mapper.Map<CategoryDto>(_category);
            response.StatusCode = StatusCodes.Status201Created;
            response.Message = $"Category : '{createdCategoryToReturn.Name }' Created Successfully";
            response.Data = createdCategoryToReturn;
            return CreatedAtRoute("GetCategory", new { createdCategoryToReturn.Id }, response);
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public IActionResult UpdateCategory(Guid id, [FromBody] CategoryForUpdateDto category,
            [FromQuery] bool imageChanged = false)
        {
            var response = new ResponseModel();

            #region check existance of parent category
            //check availability of parent category in the model
            try
            {
                var parentCategoryExists = repositroy.CategoryExists(category.CategoryId.Value);
                //if the repo returns null, then the given parent category id is invalid
                if (!parentCategoryExists)
                {
                    ModelState.AddModelError("CategoryId",
                        "The given parent category id doesn't exist");
                }
            }
            catch (Exception e)
            {
                //check if the reason of exception is that the parent category is null
                if (e.GetType() == typeof(InvalidOperationException))
                {
                    //if that's the case then we got nothing to do
                }
                else
                {
                    throw;
                }
            }
            #endregion
            #region check validity of model
            if (!ModelState.IsValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid Category Format";
                response.Data = ModelState;
                return BadRequest(response);
            }
            #endregion
            #region check existance of category
            if (!repositroy.CategoryExists(id))
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = ($"Category Not Found");
                return NotFound(response);
            }
            #endregion
            #region check dublication
            //check if there's a category with the same name and parent as the newly edited category
            var _category = repositroy.GetCategoryByNameAndSuperCategory(category.Name, category.NameAR, category.CategoryId);
            //if the categoryforedit and existing category in database has different id that would result in a dublication
            //if they have the same id, then we are merely editing the existing category without changing its name and parent
            if (_category != null && _category.Id != id)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = $"Category: {category.Name} already exists";
                return BadRequest(response);
            }
            #endregion
            #region check cyclic relation between cateogry and parent
            // a category cannot be a parent of its parent
            if (category.CategoryId == id)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = $"Parent Category cannot be the exact same category as child";
                return BadRequest(response);
            }
            #endregion

            //get the original category from the database with tracking on
            _category = repositroy.GetCategoryById(id,language:"all");
            //copy the content of the edited category into the original category
            mapper.Map(category, _category);
            //if the image was changed, upload the new image and save its name in the original category
            #region check image change
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
            _category.Names = new List<Name>()
            {
                new Name(){Value = category.Name, Culture = "en"},
                new Name(){Value = category.NameAR, Culture = "ar"}
            };
            repositroy.UpdateCategory(id, _category);
            repositroy.Save();
            return NoContent();
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public IActionResult DeleteCategory(Guid id)
        {
            var response = new ResponseModel();
            #region check existance of category
            var category = repositroy.GetCategoryById(id);
            if (category == null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Category to be deleted not found !";
                return NotFound(response);
            }
            #endregion
            #region check if the category is parent
            if (repositroy.IsSuperCategory(id))
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Category has childred";
                return BadRequest(response);
            }
            #endregion

            //if the operation is valid check if an image exists for category, if there's one
            //delete it too
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

        private string CreateCategoriesResourceUri(CategoryResourceParameters categoryParameters,
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
