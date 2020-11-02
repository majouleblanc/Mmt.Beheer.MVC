using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.ViewModels.MmtUser
{
    public class MmtUserPutViewModel
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Gebruikersnaam mag niet langer dan 255 charakters zijn")]
        public virtual string UserName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Voornaam mag niet langer dan 50 charakters zijn")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(120, ErrorMessage = "Achternaam mag niet langer dan 120 charakters zijn")]
        public string LastName { get; set; }

        [EmailAddress]
        [Required]
        [MaxLength(255, ErrorMessage = "E-mail mag niet langer dan 255 charakters zijn")]
        public virtual string Email { get; set; }

        [Required]
        public virtual bool EmailConfirmed { get; set; }


        [DataType(DataType.Password)]
        [MaxLength(255, ErrorMessage = "Wachtwoord mag niet langer dan 255 charakters zijn")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [MaxLength(255, ErrorMessage = "Confirm Wachtwoord mag niet langer dan 255 charakters zijn")]
        [Compare("Password", ErrorMessage = "Password and confirm password doesnt match")]
        public string ConfirmPassword { get; set; }

        [MaxLength(255, ErrorMessage = "Straat mag niet langer dan 255 charakters zijn")]
        public string Street { get; set; }

        [MaxLength(20, ErrorMessage = "Postcode mag niet langer dan 20 charakters zijn")]
        public string PostalCode { get; set; }

        [MaxLength(80, ErrorMessage = "Stad mag niet langer dan 80 charakters zijn")]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [MaxLength(25, ErrorMessage = "Mobieltje mag niet langer dan 25 charakters zijn")]
        public virtual string Mobile { get; set; }

        [MaxLength(25, ErrorMessage = "Telefoon thuis mag niet langer dan 25 charakters zijn")]
        public string PhoneHome { get; set; }

        [MaxLength(25, ErrorMessage = "Telefoon werk mag niet langer dan 25 charakters zijn")]
        public string PhoneWork { get; set; }

        [MaxLength(255, ErrorMessage = "Functie mag niet langer dan 255 charakters zijn")]
        public string Function { get; set; }

        public List<Mmt.Beheer.MVC.Models.Role> Roles { get; set; }

        public List<string> SelectedRolesIds { get; set; }
        public SelectList RolesSelectList { get; set; }
    }
}
