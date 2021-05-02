using AutoMapper;
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
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ISpotsRepositroy repositroy;

        public CategoryController(IMapper mapper, ISpotsRepositroy repositroy)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = repositroy.GetCategories();
            return Ok(mapper.Map<IEnumerable<CategoryDto>>(categories));
        }

        [HttpGet("id", Name = "GetCategory")]
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
        [HttpGet("name")]
        public IActionResult GetCategoryByName(string name)
        {
            var category = repositroy.GetCategoryByName(name);
            if (category != null)
            {
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
            var _category = repositroy.GetCategoryByName(category.Name);
            if(_category != null)
            {
                return BadRequest($"Category: {category.Name} already exists");
            }
            if (!string.IsNullOrWhiteSpace(category.SuperCategoryName))
            {
                var superCategory = repositroy.GetCategoryByName(category.SuperCategoryName);
                _category.SuperCategory = superCategory;
            }
            _category.Name = category.Name;
            _category.Description = category.Description;

            repositroy.AddCategory(_category);
            repositroy.Save();

            var createdCategoryToReturn = mapper.Map<CategoryDto>(_category);
            return CreatedAtRoute("GetCategory", new { createdCategoryToReturn.Id }, createdCategoryToReturn);

        }

        [HttpPut("id")]
        public IActionResult UpdateCategory(Guid id, [FromBody] CategoryForUpdateDto category)
        {
            if (!repositroy.CategoryExists(id))
            {
                return NotFound();
            }
            var _category = repositroy.GetCategoryByName(category.Name);
            if(_category != null)
            {
                return BadRequest($"Category: {category.Name} already exists");
            }
            _category = repositroy.GetCategoryById(id);
            if (string.IsNullOrEmpty(category.SuperCategoryName))
            {
                _category.SuperCategory = null;
            }
            else
            {
                var superCategory = repositroy.GetCategoryByName(category.SuperCategoryName);
                _category.SuperCategory = superCategory;
            }
            _category.Name = category.Name;
            _category.Description = category.Description;

            repositroy.UpdateCategory(id, _category);
            repositroy.Save();

            return NoContent();
        }

        [HttpDelete("id")]
        public IActionResult DeleteCategory(Guid id)
        {
            var category = repositroy.GetCategoryById(id);
            if(category == null)
            {
                return NotFound();
            }
            repositroy.DeleteCategory(category);
            repositroy.Save();
            return NoContent();
        }
    }
}
