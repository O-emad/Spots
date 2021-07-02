using AutoMapper;
using ExtraSW.IDP.Entities;
using Marvin.IDP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spots.APIs;
using Spots.DTO;
using Spots.Services.Helpers;
using Spots.Services.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExtraSW.IDP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserManageController : ControllerBase
    {
        private readonly ILocalUserService localUserService;
        private readonly IMapper mapper;

        public UserManageController(ILocalUserService localUserService, IMapper mapper)
        {
            this.localUserService = localUserService ?? throw new ArgumentNullException(nameof(localUserService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet(Name = "GetUsers")]
        public IActionResult GetUsers([FromQuery] IndexResourceParameters userParameters)
        {
            var users = localUserService.GetUsers(userParameters);
            var previousPageLink = users.HasPrevious ?
                CreateUsersResourceUri(userParameters, ResourceUriType.PreviousPage) : null;

            var nextPageLink = users.HasNext ?
                CreateUsersResourceUri(userParameters, ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = users.TotalCount,
                pageSize = users.PageSize,
                currentPage = users.CurrentPage,
                totalPages = users.TotalPages,
                previousPageLink,
                nextPageLink
            };

            var paginationForMVC = new
            {
                searchQuery = userParameters.SearchQuery,
                currentPage = users.CurrentPage,
                totalPages = users.TotalPages,
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
                Data = mapper.Map<IEnumerable<UserDto>>(users).ToList()
            });
        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpGet("{id}",Name ="GetUser")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            if (User.IsInRole("Vendor"))
            {
                var userSub = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                var user = await localUserService.GetUserBySubjectAsync(userSub);
                id = user.Id;
            }
            if (localUserService.UserExists(id))
            {
                var user = await localUserService.GetUserByIdAsync(id);
                var users = new List<User>();
                users.Add(user);
                return Ok(new ResponseModel()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "",
                    Data = mapper.Map<IEnumerable<UserDto>>(users).ToList()
                }); ;
            }
            else
            {
                return NotFound(new ResponseModel()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "User Not Found",
                    Data = new { }
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]UserForCreationDto user)
        {
            var response = new ResponseModel();

            if (!ModelState.IsValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid User Format";
                return BadRequest(response);
            }
            var _user = await localUserService.GetUserByUserNameAsync(user.UserName);
            if(_user != null)
            {
                response.StatusCode = StatusCodes.Status409Conflict;
                response.Message = $"User: {user.UserName} already exists";
                return Conflict(response);
            }
            _user = mapper.Map<User>(user);
            localUserService.AddUser(_user);
            await localUserService.SaveChangesAsync();
            var createdUserToReturn = mapper.Map<UserDto>(_user);
            response.StatusCode = StatusCodes.Status201Created;
            response.Message = $"User : '{createdUserToReturn.UserName }' Created Successfully";
            response.Data = createdUserToReturn;
            return CreatedAtRoute("GetUser", new { createdUserToReturn.Id }, response);
        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser([FromBody]UserForUpdateDto user, Guid id)
        {
            var response = new ResponseModel();

            if (!ModelState.IsValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid User Format";
                return BadRequest(response);
            }
            var _user = await localUserService.GetUserByIdAsync(id);
            if(_user == null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "User not found";
                return NotFound(response);
            }
            var role = _user.Claims.FirstOrDefault(c => c.Type == "role");
            if(role != null)
            {
                var _role = mapper.Map<UserClaimForCreation>(role);
                user.Claims.Add(_role);
            }
            mapper.Map(user, _user);
            //foreach(var claim in user.Claims)
            //{
            //    if(_user.Claims.)
            //}
            await localUserService.SaveChangesAsync();
            return NoContent();

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await localUserService.GetUserByIdAsync(id);
            if(user == null)
            {
                return NotFound(new ResponseModel() { StatusCode = StatusCodes.Status404NotFound, Message = "User not found" });
            }
            localUserService.DeleteUser(user);
            await localUserService.SaveChangesAsync();
            return NoContent();
        }

        private string CreateUsersResourceUri(IndexResourceParameters usersParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetUsers", new
                    {
                        pageNumber = usersParameters.PageNumber - 1,
                        pageSize = usersParameters.PageSize,
                        seachQuery = usersParameters.SearchQuery
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetUsers", new
                    {
                        pageNumber = usersParameters.PageNumber + 1,
                        pageSize = usersParameters.PageSize,
                        seachQuery = usersParameters.SearchQuery
                    });
                default:
                    return Url.Link("GetUsers", new
                    {
                        pageNumber = usersParameters.PageNumber,
                        pageSize = usersParameters.PageSize,
                        seachQuery = usersParameters.SearchQuery
                    });
            }
        }
    }
}
