using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Authorize(Roles = "Admin")]
    public class SettingController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ISpotsRepositroy repositroy;

        public SettingController(IMapper mapper, ISpotsRepositroy repositroy)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.repositroy = repositroy ?? throw new ArgumentNullException(nameof(repositroy));
        }

        [HttpGet(Name  = "GetSettings")]
        public IActionResult GetSetting()
        {
            var response = new ResponseModel();
            var setting = repositroy.GetSetting();
            if(setting == null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Setting doesn't exist";
                response.Data = new { };
                return NotFound(response);
            }
            var settings = new List<Setting>();
            settings.Add(setting);
            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "";
            response.Data = mapper.Map<IEnumerable<SettingDto>>(settings);
            return Ok(response);
        }

        [HttpPut]
        public IActionResult UpdateSetting([FromBody] SettingForUpdateDto newSetting)
        {
            var response = new ResponseModel();
            if (!ModelState.IsValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Bad Settings format";
                response.Data = new { };
                return BadRequest(response);
            }
            var setting = repositroy.GetSetting();
            if (setting == null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Setting doesn't exist";
                response.Data = new { };
                return NotFound(response);
            }
            mapper.Map(newSetting, setting);
            repositroy.UpdateSetting();
            repositroy.Save();
            return NoContent();
        }

    }
}
