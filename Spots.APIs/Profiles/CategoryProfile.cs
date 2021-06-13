using AutoMapper;
using Spots.Domain;
using Spots.DTO;
using Spots.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.APIs.Profiles
{
    public class CategoryProfile :Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>();
            //CreateMap<PagedList<Category>, PagedList<CategoryDto>>();
            CreateMap<CategoryForCreationDto, Category>();
            CreateMap<CategoryForUpdateDto, Category>();
        }
    }
}
