using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.ViewModels.Comment
{
    public class CommentPostViewModel
    {
        [Required]
        public int CuriosityId { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Gebruikersnaam mag niet langer dan 255 charakters zijn")]
        public string UserName { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Commentaar mag niet langer dan 50 charakters zijn")]
        public string CommentMsg { get; set; }

        [EmailAddress]
        [MaxLength(120, ErrorMessage = "Email mag niet langer dan 120 charakters zijn")]
        public string Email { get; set; }
    }
}
