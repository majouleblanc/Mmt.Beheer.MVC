using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.ViewModels.Photo
{
    public class PhotoPostViewModel
    {

        [Required]
        public int CuriosityId { get; set; }

        [Required]
        public IFormFile Photo { get; set; }
    }
}
