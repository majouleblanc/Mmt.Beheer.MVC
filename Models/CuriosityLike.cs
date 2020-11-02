using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.Models
{
    public class CuriosityLike
    {
        public int Id { get; set; }

        [Required]
        public int CuriosityId { get; set; }
        public long Likes { get; set; }

        public CuriosityLike() { }

        public CuriosityLike(int id)
        {
            this.CuriosityId = id;
            this.Likes = 0;
        }
    }
}
