using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mmt.Beheer.MVC.Extensions;
using Mmt.Beheer.MVC.Models;
using Mmt.Beheer.MVC.Services;
using Mmt.Beheer.MVC.ViewModels.Role;
using Newtonsoft.Json;

namespace Mmt.Beheer.MVC.Controllers
{
    [Route("[Controller]/[Action]")]
    public class RolesController : Controller
    {
        private readonly IConfiguration _Configuration;
        private readonly IMapper _Mapper;
        private readonly IRoleServices _RoleServices;

        public string BaseUrl { get; private set; }
        public RolesController(IConfiguration configuration, IMapper mapper, IRoleServices roleServices)
        {
            _Configuration = configuration;
            _Mapper = mapper;
            _RoleServices = roleServices;
            BaseUrl = _Configuration["BaseUrl"];
        }
        // GET: RoleController
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource Curiosities using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/Roles");
            //Checking the response is successful or not which is sent using HttpClient  
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the Curiosity list  
                var roles = JsonConvert.DeserializeObject<List<Role>>(response);
                //returning the curiosity list to view  
                //return Ok(Res.Content.ReadAsStringAsync().Result);
                return View(roles);

            }
            return View();

        }

        [Route("{id}")]
        // GET: RoleController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource GetAllCuriosities using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/Roles/" + id);
            //Checking the response is successful or not which is sent using HttpClient  
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the Curiosity list  
                var role = JsonConvert.DeserializeObject<Role>(response);

                var usersInRole =await _RoleServices.GetUsersInRole(role.Id, User);
                foreach (var user in usersInRole)
                {
                    user.Roles = await _RoleServices.GetRolesForUserWith(user.Id, User);
                }

                ViewData["Users"] = usersInRole;

                return View(role);
            }
            return View();
        }

        // GET: RoleController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] RolePostViewModel model/*IFormCollection collection*/)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "data not valid");
                return View(ModelState);
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);


            var multiContent = new MultipartFormDataContent();

            multiContent.Add(new StringContent(model.Name), "RoleName");


            var response = await client.PostAsync(BaseUrl + "api/Roles/", multiContent);
            if (response.IsSuccessStatusCode)
            {
                var retValue = await response.Content.ReadAsStreamAsync();
                return RedirectToAction(nameof(Index));
            }
            //else
            //{
            //    return BadRequest(response.Content.ReadAsStringAsync().Result);
            //}

            return View();

        }

        // GET: RoleController/Edit/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "invalid Tour id");
                return View();
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage resp = await client.GetAsync(BaseUrl + "api/Roles/" + id);
            if (resp.IsSuccessStatusCode)
            {
                var response = await resp.Content.ReadAsStringAsync();

                var role = JsonConvert.DeserializeObject<RolePutViewModel>(response);
                var model = _Mapper.Map<RolePutViewModel>(role);
                ViewData["id"] = id;
                return View(model);
            }
            return View();
        }

        // POST: RoleController/Edit/5
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, [FromForm] RolePutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var client = new HttpClient();
            client.AddTokens(User);

            client.DefaultRequestHeaders.Clear();

            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            multiContent.Add(new StringContent(model.Name), "RoleName");

            var response = await client.PutAsync(BaseUrl + $"api/Roles/{id}", multiContent);
            if (response.IsSuccessStatusCode)
            {
                var retValue = await response.Content.ReadAsStreamAsync();
                try
                {
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            //else
            //{
            //    return BadRequest(response.Content.ReadAsStringAsync().Result);
            //}
            return View(model);
        }

        // POST: RoleController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", $"Tour Id not valid");
                return View(ModelState);
            }

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //HTTP DELETE
            HttpResponseMessage Res = await client.DeleteAsync(BaseUrl + "api/Roles/" + id);

            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the curiosity object  
                //var curiosity = JsonConvert.DeserializeObject<Curiosity>(response);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
