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
using Newtonsoft.Json;

namespace Mmt.Beheer.MVC.Controllers
{
    [Route("[Controller]/[Action]")]
    public class LogsController : Controller
    {
        private readonly IConfiguration _Configuration;
        private readonly IMapper _Mapper;

        public string BaseUrl { get; set; }
        public LogsController(IConfiguration configuration, IMapper mapper)
        {
            _Configuration = configuration;
            _Mapper = mapper;

            BaseUrl = _Configuration["BaseUrl"];
        }

        // GET: LogsController
        public async Task<ActionResult> Index()
        {
            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource  using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/Logs");
            //Checking the response is successful or not which is sent using HttpClient  
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the tours list  
                var logs = JsonConvert.DeserializeObject<List<Log>>(response);
                //returning the curiosity list to view  
                return View(logs);

            }
            return View();
        }

        // GET: LogsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource  using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/Logs/" + id);
            //Checking the response is successful or not which is sent using HttpClient  
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the tours list  
                var log = JsonConvert.DeserializeObject<Log>(response);
                return View(log);
            }
            return View();
        }

        // GET: LogsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LogsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] Log model/*IFormCollection collection*/)
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

            multiContent.Add(new StringContent(model.UserName), nameof(model.UserName));
            multiContent.Add(new StringContent(model.Email ?? ""), nameof(model.Email));
            multiContent.Add(new StringContent(model.Role ?? ""), nameof(model.Role));
            multiContent.Add(new StringContent(model.Longitude ?? ""), nameof(model.Longitude));
            multiContent.Add(new StringContent(model.Latitude ?? ""), nameof(model.Latitude));
            multiContent.Add(new StringContent(model.CuriosityName ?? ""), nameof(model.CuriosityName));
            multiContent.Add(new StringContent(model.CuriosityId.ToString() ?? ""), "CuriosityId");
            multiContent.Add(new StringContent(model.CuriosityLongitude ?? ""), nameof(model.CuriosityLongitude));
            multiContent.Add(new StringContent(model.CuriosityLatitude ?? ""), nameof(model.CuriosityLatitude));
            multiContent.Add(new StringContent(model.InsertedOn.ToString() ?? ""), "InsertedOn");


            var response = await client.PostAsync(BaseUrl + "api/Logs/", multiContent);
            if (response.IsSuccessStatusCode)
            {
                var retValue = await response.Content.ReadAsStreamAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                BadRequest(response.StatusCode);
            }

            return View();
        }

        // GET: LogsController/Edit/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("", "invalid Tour id");
                return View();
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage resp = await client.GetAsync(BaseUrl + "api/Logs/" + id);
            if (resp.IsSuccessStatusCode)
            {
                var response = await resp.Content.ReadAsStringAsync();

                var model = JsonConvert.DeserializeObject<Log>(response);
                //var model = _Mapper.Map<CuriositiesPutViewModel>(curiosity);
                ViewData["id"] = id;
                return View(model);
            }
            return View();
        }

        // POST: LogsController/Edit/5
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [FromForm] Log model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);


            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(new StringContent(model.Id.ToString()), nameof(model.Id));
            multiContent.Add(new StringContent(model.UserName), nameof(model.UserName));
            multiContent.Add(new StringContent(model.Email ?? ""), nameof(model.Email));
            multiContent.Add(new StringContent(model.Role ?? ""), nameof(model.Role));
            multiContent.Add(new StringContent(model.Longitude ?? ""), nameof(model.Longitude));
            multiContent.Add(new StringContent(model.Latitude ?? ""), nameof(model.Latitude));
            multiContent.Add(new StringContent(model.CuriosityName ?? ""), nameof(model.CuriosityName));
            multiContent.Add(new StringContent(model.CuriosityId.ToString() ?? ""), "CuriosityId");
            multiContent.Add(new StringContent(model.CuriosityLongitude ?? ""), nameof(model.CuriosityLongitude));
            multiContent.Add(new StringContent(model.CuriosityLatitude ?? ""), nameof(model.CuriosityLatitude));
            multiContent.Add(new StringContent(model.InsertedOn.ToString() ?? ""), "InsertedOn");

            var response = await client.PutAsync(BaseUrl + $"api/Logs/{id}", multiContent);
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
            return View(model);
        }      

        // POST: LogsController/Delete/5
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("", $"Tour Id not valid");
                return View(ModelState);
            }

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);


            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //HTTP DELETE
            HttpResponseMessage Res = await client.DeleteAsync(BaseUrl + "api/Logs/" + id);

            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the Log obejct  
                var log = JsonConvert.DeserializeObject<Log>(response);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
