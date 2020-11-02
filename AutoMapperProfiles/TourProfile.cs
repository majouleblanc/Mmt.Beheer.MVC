using AutoMapper;
using Mmt.Beheer.MVC.Models;
using Mmt.Beheer.MVC.ViewModels.Tours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.AutoMapperProfiles
{
    public class TourProfile : Profile
    {
        public TourProfile()
        {
            CreateMap<Tour, ToursPutViewModel>();
        }
    }
}
