using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.Models
{
    public class HomePageElement
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public string imagePath { get; set; }
        public List<string> ForRoles { get; set; }
    }
}
