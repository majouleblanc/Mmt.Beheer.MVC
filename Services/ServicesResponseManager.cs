using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.Services
{
    

    public class ServicesResponseManager
    {
        public string Result { get; set; }
        public bool IsSucces { get; set; }
        public List<string> Roles { get; set; }
        public IEnumerable<string> Errors { get; set; }


        public DateTime? ExpireDate { get; set; }
    }
}
