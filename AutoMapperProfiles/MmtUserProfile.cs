using AutoMapper;
using Mmt.Beheer.MVC.Models;
using Mmt.Beheer.MVC.ViewModels.MmtUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.AutoMapperProfiles
{
    public class MmtUserProfile : Profile
    {
        public MmtUserProfile()
        {
            CreateMap<MmtUser, MmtUserPutViewModel>();
        }
    }
}
