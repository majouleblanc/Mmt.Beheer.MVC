using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.Models
{
    public class Log
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Gebruikersnaam mag niet langer dan 255 charakters zijn")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(50, ErrorMessage = "E-mail mag niet langer dan 50 charakters zijn")]
        public string Email { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Rol mag niet langer dan 50 charakters zijn")]
        public string Role { get; set; }

        [Required]
        [MaxLength(25, ErrorMessage = "Lengtegraad mag niet langer dan 25 charakters zijn")]
        public string Longitude { get; set; }

        [Required]
        [MaxLength(25, ErrorMessage = "Breedtegraad mag niet langer dan 25 charakters zijn")]
        public string Latitude { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Bezienswaardigheidnaam mag niet langer dan 255 charakters zijn")]
        public string CuriosityName { get; set; }

        [Required]
        public int CuriosityId { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Bezienswaardigheidlengtegraad mag niet langer dan 255 charakters zijn")]
        public string CuriosityLongitude { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Bezienswaardigheidbreedtegraad mag niet langer dan 255 charakters zijn")]
        public string CuriosityLatitude { get; set; }

        [Required]
        public DateTime InsertedOn { get; set; }
    }
}
