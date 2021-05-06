﻿using AutoMapper;
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
            var _category = repositroy.GetCategoryByName(category.Name);
            if (_category != null)
            {
                return BadRequest($"Category: {category.Name} already exists");
            }
            _category = mapper.Map<Category>(category);
            if (repositroy.CategoryExists(category.SuperCategoryId))
            {
                _category.SuperCategoryId = category.SuperCategoryId;
            }


            repositroy.AddCategory(_category);
            repositroy.Save();

            var createdCategoryToReturn = mapper.Map<CategoryDto>(_category);
            return CreatedAtRoute("GetCategory", new { createdCategoryToReturn.Id }, createdCategoryToReturn);

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
