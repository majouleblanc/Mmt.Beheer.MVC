using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.Models
{
    public class MmtUser
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Voornaam mag niet langer dan 50 charakters zijn")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(120, ErrorMessage = "Achternaam mag niet langer dan 120 charakters zijn")]
        public string LastName { get; set; }

        [MaxLength(255, ErrorMessage = "Straat mag niet langer dan 255 charakters zijn")]
        public string Street { get; set; }

        [MaxLength(20, ErrorMessage = "Postcode mag niet langer dan 20 charakters zijn")]
        public string PostalCode { get; set; }

        [MaxLength(80, ErrorMessage = "Stad mag niet langer dan 80 charakters zijn")]
        public string City { get; set; }

        public string Country { get; set; }

        [MaxLength(25, ErrorMessage = "Telefoon thuis mag niet langer dan 25 charakters zijn")]
        public string PhoneHome { get; set; }

        [MaxLength(25, ErrorMessage = "Telefoon werk mag niet langer dan 25 charakters zijn")]
        public string PhoneWork { get; set; }

        [MaxLength(255, ErrorMessage = "Functie mag niet langer dan 255 charakters zijn")]
        public string Function { get; set; }

        public virtual DateTimeOffset? LockoutEnd { get; set; }
        
        public virtual bool PhoneNumberConfirmed { get; set; }
        
        public virtual string PhoneNumber { get; set; }
        
        public virtual bool EmailConfirmed { get; set; }
        
        public virtual string NormalizedEmail { get; set; }
        
        public virtual string Email { get; set; }
        
        public virtual string NormalizedUserName { get; set; }
        
        public virtual string UserName { get; set; }
        
        public virtual string Id { get; set; }
        
        public virtual bool LockoutEnabled { get; set; }
        
        public virtual int AccessFailedCount { get; set; }

        public List<Role> Roles { get; set; }
    }
}
