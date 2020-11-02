using AutoMapper;
using Mmt.Beheer.MVC.Models;
using Mmt.Beheer.MVC.ViewModels.Curiosities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.AutoMapperProfiles
{
    public class CuriosityProfile : Profile
    {
        public CuriosityProfile()
        {
            CreateMap<Curiosity, CuriositiesPutViewModel>();
        }
    }
}
