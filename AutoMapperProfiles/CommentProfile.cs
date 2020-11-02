using AutoMapper;
using Mmt.Beheer.MVC.Models;
using Mmt.Beheer.MVC.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.AutoMapperProfiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentPutViewModel>();
        }
    }
}
