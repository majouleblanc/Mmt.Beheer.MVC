using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Mmt.Beheer.MVC.Extensions;
using Mmt.Beheer.MVC.Models;
using Mmt.Beheer.MVC.Services;
using Mmt.Beheer.MVC.ViewModels.MmtUser;
using Newtonsoft.Json;

namespace Mmt.Beheer.MVC.Controllers
{
    [Route("[Controller]/[Action]")]
    public class MmtUsersController : Controller
    {
        private readonly IConfiguration _Configuration;
        private readonly IMapper _Mapper;
        private readonly IRoleServices _RoleServices;

        public string BaseUrl { get; }

        public MmtUsersController(IConfiguration configuration, IMapper mapper, IRoleServices roleServices)
        {
            _Configuration = configuration;
            _Mapper = mapper;
            _RoleServices = roleServices;
            BaseUrl = _Configuration["BaseUrl"];
        }

        [HttpGet]
        // GET: MmtUsersController
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource Curiosities using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/MmtUser/MmtUsers");
            //Checking the response is successful or not which is sent using HttpClient  
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the Curiosity list  
                var mmtUsers = JsonConvert.DeserializeObject<List<MmtUser>>(response);

                foreach (var user in mmtUsers)
                {
                    user.Roles = await _RoleServices.GetRolesForUserWith(user.Id, User);
                }
                //returning the curiosity list to view  
                return View(mmtUsers);

            }
            return View();
        }

        // GET: MmtUsersController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource GetAllCuriosities using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/MmtUser/" + id);
            //Checking the response is successful or not which is sent using HttpClient  
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the Curiosity list  
                var mmtUser = JsonConvert.DeserializeObject<MmtUser>(response);
                return View(mmtUser);
            }
            return View();
        }

        // GET: MmtUsersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MmtUsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] MmtUserPostViewModel model/*IFormCollection collection*/)
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

            multiContent.Add(new StringContent(model.FirstName), nameof(model.FirstName));
            multiContent.Add(new StringContent(model.LastName ?? ""), nameof(model.LastName));
            multiContent.Add(new StringContent(model.Street ?? ""), nameof(model.Street));
            multiContent.Add(new StringContent(model.PostalCode ?? ""), nameof(model.PostalCode));
            multiContent.Add(new StringContent(model.City ?? ""), nameof(model.City));
            multiContent.Add(new StringContent(model.Country ?? ""), nameof(model.Country));
            multiContent.Add(new StringContent(model.PhoneHome ?? ""), nameof(model.PhoneHome));
            multiContent.Add(new StringContent(model.PhoneWork ?? ""), nameof(model.PhoneWork));
            multiContent.Add(new StringContent(model.Function ?? ""), nameof(model.Function));
            multiContent.Add(new StringContent(model.Mobile ?? ""), nameof(model.Mobile));
            multiContent.Add(new StringContent(model.Email ?? ""), nameof(model.Email));
            multiContent.Add(new StringContent(model.EmailConfirmed == true ? "true" : "false"), "ConfirmEmail");
            multiContent.Add(new StringContent(model.UserName ?? ""), nameof(model.UserName));
            multiContent.Add(new StringContent(model.Password ?? ""), nameof(model.Password));
            multiContent.Add(new StringContent(model.ConfirmPassword ?? ""), nameof(model.ConfirmPassword));
            multiContent.Add(new StringContent(model.RoleName ?? ""), "Roles");


            var response = await client.PostAsync(BaseUrl + "api/MmtUser/PostMmtUser", multiContent);
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

        // GET: MmtUsersController/Edit/5
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

            HttpResponseMessage resp = await client.GetAsync(BaseUrl + "api/MmtUser/" + id);
            if (resp.IsSuccessStatusCode)
            {
                var response = await resp.Content.ReadAsStringAsync();

                var mmtUser = JsonConvert.DeserializeObject<MmtUser>(response);
                var model = _Mapper.Map<MmtUserPutViewModel>(mmtUser);


                var roles = await _RoleServices.GetAllRoles(User);
                var userRoles = model.Roles;
                /*await _CuriosityServices.GetToursForCuriosity(id)*/
                ;
                model.SelectedRolesIds = userRoles.Select(r => r.Id).ToList();

                model.RolesSelectList = new SelectList(roles, "Id", "Name", model.SelectedRolesIds);


                ViewData["id"] = id;
                return View(model);
            }
            return View();
        }

        // POST: MmtUsersController/Edit/5
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, [FromForm] MmtUserPutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);


            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(new StringContent(model.FirstName), nameof(model.FirstName));
            multiContent.Add(new StringContent(model.LastName ?? ""), nameof(model.LastName));
            multiContent.Add(new StringContent(model.Street ?? ""), nameof(model.Street));
            multiContent.Add(new StringContent(model.PostalCode ?? ""), nameof(model.PostalCode));
            multiContent.Add(new StringContent(model.City ?? ""), nameof(model.City));
            multiContent.Add(new StringContent(model.Country ?? ""), nameof(model.Country));
            multiContent.Add(new StringContent(model.PhoneHome ?? ""), nameof(model.PhoneHome));
            multiContent.Add(new StringContent(model.PhoneWork ?? ""), nameof(model.PhoneWork));
            multiContent.Add(new StringContent(model.Function ?? ""), nameof(model.Function));
            multiContent.Add(new StringContent(model.Mobile ?? ""), nameof(model.Mobile));
            multiContent.Add(new StringContent(model.Email ?? ""), nameof(model.Email));
            multiContent.Add(new StringContent(model.EmailConfirmed == true ? "true" : "false"), "EmailConfirmed");
            multiContent.Add(new StringContent(model.UserName ?? ""), nameof(model.UserName));
            multiContent.Add(new StringContent(model.Password ?? ""), nameof(model.Password));
            multiContent.Add(new StringContent(model.ConfirmPassword ?? ""), nameof(model.ConfirmPassword));

            if (!model.SelectedRolesIds.Any())
            {
                model.SelectedRolesIds = new List<string>();
            }

            //fetch role list from api
            var allRoles = await _RoleServices.GetAllRoles(User);

            //the api accepts a list of role names in the Roles field
            List<string> roles = new List<string>();
            Role role = null;

            //iterating over the selected role id add the name of that role in the roles we gonna send to the api
            foreach (var SelectedRoleId in model.SelectedRolesIds)
            {
                role = allRoles.FirstOrDefault(r=>r.Id == SelectedRoleId);
                roles.Add(role.Name);
                multiContent.Add(new StringContent(role.Name ?? ""), nameof(model.Roles));
            }


            var response = await client.PutAsync(BaseUrl + $"api/MmtUser/{id}", multiContent);
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
            //return BadRequest(response.Content.ReadAsStringAsync().Result);
            return View(model);
        }

        // POST: MmtUsersController/Delete/5
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
            HttpResponseMessage Res = await client.DeleteAsync(BaseUrl + "api/MmtUser/" + id);

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
