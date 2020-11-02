using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mmt.Beheer.MVC.Models;

namespace Mmt.Beheer.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public List<HomePageElement> HomePageElements { get; set; }

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _WebHostEnvironment = webHostEnvironment;

            HomePageElements = new List<HomePageElement>
            {
                new HomePageElement{Id =1, EntityName= "Tours", imagePath = "Tours.jpg", ForRoles=new List<string>{"DIRECTOR","SUPERVISOR" } },
                new HomePageElement{Id =2, EntityName= "Curiosities", imagePath = "Curiosities.jpg", ForRoles=new List<string>{"DIRECTOR","SUPERVISOR" } },
                //new HomePageElement{Id =3, EntityName= "Comments", imagePath = "Comments.jpg" },
                new HomePageElement{Id =4, EntityName= "MmtUsers", imagePath = "Users.jpg", ForRoles=new List<string>{"DIRECTOR" } },
                new HomePageElement{Id =5, EntityName= "Roles", imagePath = "Roles.jpg", ForRoles=new List<string>{"DIRECTOR"} },
                //new HomePageElement{Id =6, EntityName= "Photo", imagePath = "Photos.jpg" },
                new HomePageElement{Id =7, EntityName= "Logs", imagePath = "Logs.jpg", ForRoles=new List<string>{"DIRECTOR","SUPERVISOR"  } }
            };
        }

        public IActionResult Index()
        {
            //check if the user have the supervisor claim
            if (User.Claims.Any(c => c.Value == "DIRECTOR"))
            {
                return View(HomePageElements.Where(e => e.ForRoles.Contains("DIRECTOR")));
                //return View();
            }
            if (User.Claims.Any(c => c.Value == "SUPERVISOR"))
            {
                return View(HomePageElements.Where(e => e.ForRoles.Contains("SUPERVISOR")));
            }

            return Unauthorized();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
