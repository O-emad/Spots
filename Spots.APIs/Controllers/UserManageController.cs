using AutoMapper;
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
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserManageController : ControllerBase
    {
        private readonly ILocalUserService localUserService;
        private readonly IMapper mapper;

        public UserManageController(ILocalUserService localUserService, IMapper mapper)
        {
            this.localUserService = localUserService ?? throw new ArgumentNullException(nameof(localUserService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult GetUsers([FromQuery] IndexResourceParameters userParameters)
        {
            //var users = localUserService.GetUsers();
            //return Ok(users);
            var users = localUserService.GetUsers(userParameters);
            //var users = new PagedList<UserDto>(new List<UserDto>() { }, 1, 1, 1);
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
