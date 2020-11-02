using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.ViewModels.Role
{
    public class RolePostViewModel
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Rol mag niet langer dan 50 charakters zijn")]
        public string Name { get; set; }
    }
}
