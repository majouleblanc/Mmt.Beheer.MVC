using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.Models
{
    public class Role
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
