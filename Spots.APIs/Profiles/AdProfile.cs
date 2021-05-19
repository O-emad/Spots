using AutoMapper;
using Spots.Domain;
using Spots.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.APIs.Profiles
{
    public class AdProfile :Profile
    {
        public AdProfile()
        {
            CreateMap<Ad, AdDto>();
            CreateMap<AdForCreationDto, Ad>();
            CreateMap<AdForUpdateDto, Ad>();
        }
        
    }
}
