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
    public class VendorProfile :Profile
    {
        public VendorProfile()
        {
            CreateMap<Vendor, VendorDto>()
                .ForMember(dest => dest.Follows, opt => opt.MapFrom(src => src.Follows.Count));
            CreateMap<VendorForCreationDto, Vendor>();
            CreateMap<VendorForUpdateDto, Vendor>()
                .ForMember(dest => dest.OwnerId, opt => opt.PreCondition(c => !string.IsNullOrWhiteSpace(c.OwnerId)))
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
        }
    }
}
