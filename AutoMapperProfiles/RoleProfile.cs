using AutoMapper;
using Mmt.Beheer.MVC.Models;
using Mmt.Beheer.MVC.ViewModels.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.AutoMapperProfiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            //CreateMap<Role, RolePutViewModel>()
            //    .ForMember(dest=>dest.Name,
            //    opt=>opt.MapFrom(src => src.Name))
            //    .ReverseMap();
        }
    }
}
