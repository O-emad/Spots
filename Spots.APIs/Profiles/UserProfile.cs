using AutoMapper;
using ExtraSW.IDP.Entities;
using Spots.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.APIs.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<ExtraSW.IDP.Entities.UserClaim, Spots.DTO.UserClaim>();
            CreateMap<UserForCreationDto, User>();
            CreateMap<UserClaimForCreation, ExtraSW.IDP.Entities.UserClaim>();
        }
    }
}
