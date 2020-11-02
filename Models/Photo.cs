using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.Models
{
    public class Photo
    {
        public int Id { get; set; }

        [Required]
        public int CuriosityId { get; set; }

        public string photoUrl { get; set; }

    }
}
