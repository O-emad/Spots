using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spots.Domain;
using Spots.DTO;
using Spots.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = repositroy.GetCategories();
            return Ok(new ResponseModel() 
            { 
                StatusCode = StatusCodes.Status200OK,
                Message = "",
                Data = categories
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
            var _category = repositroy.GetCategoryByName(category.Name);

            if (!ModelState.IsValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid Category Format";
                return BadRequest(response);
            }

            if (_category != null && _category.SuperCategoryId == category.SuperCategoryId)
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

            repositroy.AddCategory(_category);
            repositroy.Save();
            var createdCategoryToReturn = mapper.Map<CategoryDto>(_category);
            response.StatusCode = StatusCodes.Status201Created;
            response.Message = $"Category : '{createdCategoryToReturn.Name }' Created Successfully";
            response.Data = createdCategoryToReturn;
            return CreatedAtRoute("GetCategory", new { createdCategoryToReturn.Id }, response);

        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(Guid id, [FromBody] CategoryForUpdateDto category)
        {
            if (!repositroy.CategoryExists(id))
            {
                return NotFound();
            }
            var _category = repositroy.GetCategoryByName(category.Name);
            if (_category != null)
            {
                return BadRequest($"Category: {category.Name} already exists");
            }
            _category = repositroy.GetCategoryById(id);
            mapper.Map(category, _category);
            if (repositroy.CategoryExists(category.SuperCategoryId))
            {
                if(category.SuperCategoryId == id)
                {
                    return BadRequest($"SuperCategory cannot be the exact same category as child");
                }
                _category.SuperCategoryId = category.SuperCategoryId;
            }
            else
            {
                _category.SuperCategoryId = Guid.Empty;
            }


            repositroy.UpdateCategory(id, _category);
            repositroy.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(Guid id)
        {
            var category = repositroy.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            if (repositroy.IsSuperCategory(id))
            {
                return BadRequest($"Category: {category.Name} is a super category, delete its children first");
            }
            repositroy.DeleteCategory(category);
            repositroy.Save();
            return NoContent();
        }
    }
}
