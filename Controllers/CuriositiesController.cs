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
using Mmt.Beheer.MVC.ViewModels.Curiosities;
using Newtonsoft.Json;

namespace Mmt.Beheer.MVC.Controllers
{
    [Route("[Controller]/[Action]")]
    public class CuriositiesController : Controller
    {
        private readonly IConfiguration _Configuration;
        private readonly IMapper _Mapper;
        private readonly ICuriosityServices _CuriosityServices;
        private readonly ITourServices _TourServices;

        public string BaseUrl { get; }

        public CuriositiesController(IConfiguration configuration, IMapper mapper, ICuriosityServices curiosityServices, ITourServices tourServices)
        {
            _Configuration = configuration;
            _Mapper = mapper;
            _CuriosityServices = curiosityServices;
            _TourServices = tourServices;
            BaseUrl = _Configuration["BaseUrl"];
        }

        [HttpGet]
        // GET: ToursController
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource Curiosities using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/Curiosities");
            //Checking the response is successful or not which is sent using HttpClient  
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the Curiosity list  
                var curiosities = JsonConvert.DeserializeObject<List<Curiosity>>(response);
                foreach (var curiosity in curiosities)
                {
                    curiosity.Tours = await _CuriosityServices.GetToursForCuriosity(curiosity.Id, User);
                    curiosity.CuriosityLike = await _CuriosityServices.GetCuriosityLike(curiosity.Id, User);
                }
                //returning the curiosity list to view  
                return View(curiosities);

            }
            return View();

        }


        // GET: ToursController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource GetAllCuriosities using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/Curiosities/" + id);
            //Checking the response is successful or not which is sent using HttpClient  
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the Curiosity list  
                var curiosity = JsonConvert.DeserializeObject<Curiosity>(response);
                curiosity.CuriosityLike = await _CuriosityServices.GetCuriosityLike(curiosity.Id, User);
                curiosity.Tours = await _CuriosityServices.GetToursForCuriosity(curiosity.Id, User);
                curiosity.Photos = await _CuriosityServices.GetCuriostyPhotos(curiosity.Id, User);
                return View(curiosity);
            }
            return View();
        }

        // GET: ToursController/Create
        public async Task<ActionResult> Create()
        {
            var model = new CuriositiesPostViewModel();
            var tours = await _TourServices.GetAllTours(User);
            //model.Curiosities = await _CuriosityServices.GetAllCuriosities();
            model.ToursSelectList = tours.Any() ? new SelectList(tours, "Id", "Name") : new SelectList("", "Id", "Name");
            return View(model);
        }

        //POST: ToursController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] CuriositiesPostViewModel model/*IFormCollection collection*/)
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

            multiContent.Add(new StringContent(model.Name), nameof(model.Name));

            if (model.Image != null)
            {
                var fileStreamContent = new StreamContent(model.Image.OpenReadStream());
                multiContent.Add(fileStreamContent, "Image", model.Image.FileName);
            }
            multiContent.Add(new StringContent(model.Coordinates ?? ""), nameof(model.Coordinates));
            multiContent.Add(new StringContent(model.Longitude ?? ""), nameof(model.Longitude));
            multiContent.Add(new StringContent(model.Latitude ?? ""), nameof(model.Latitude));
            multiContent.Add(new StringContent(model.Type ?? ""), nameof(model.Type));
            multiContent.Add(new StringContent(model.Period ?? ""), nameof(model.Period));
            multiContent.Add(new StringContent(model.Country ?? ""), nameof(model.Country));
            multiContent.Add(new StringContent(model.Region ?? ""), nameof(model.Region));
            multiContent.Add(new StringContent(model.Province ?? ""), nameof(model.Province));
            multiContent.Add(new StringContent(model.City ?? ""), nameof(model.City));
            multiContent.Add(new StringContent(model.Description ?? ""), nameof(model.Description));


            var response = await client.PostAsync(BaseUrl + "api/Curiosities/", multiContent);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(response.Content.ReadAsStringAsync().Result);
            }

            var retValue = await response.Content.ReadAsStringAsync();

            var curiosity = JsonConvert.DeserializeObject<Curiosity>(retValue);

            if (model.SelectedToursIds == null)
            {
                return RedirectToAction(nameof(Index));
            }

            //checking if the user has selected some curiosities 
            if (!model.SelectedToursIds.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            //send a post request to the api to add the curiosities for the tour
            var PostToursForCuriosityResponse = await _CuriosityServices.PostToursForCuriosity(curiosity.Id, model.SelectedToursIds, User);
            if (!PostToursForCuriosityResponse.IsSucces)
            {
                return BadRequest(PostToursForCuriosityResponse.Result);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ToursController/Edit/5
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

            HttpResponseMessage resp = await client.GetAsync(BaseUrl + "api/Curiosities/" + id);
            if (resp.IsSuccessStatusCode)
            {
                var response = await resp.Content.ReadAsStringAsync();

                var curiosity = JsonConvert.DeserializeObject<Curiosity>(response);
                CuriositiesPutViewModel model = _Mapper.Map<CuriositiesPutViewModel>(curiosity);



                var tours = await _TourServices.GetAllTours(User);
                var curiosityTours = await _CuriosityServices.GetToursForCuriosity(id, User);
                model.SelectedToursIds = curiosityTours.Select(t => t.Id).ToList();

                model.ToursSelectList = new SelectList(tours, "Id", "Name", model.SelectedToursIds);



                ViewData["id"] = id;
                return View(model);
            }
            return View();
        }

        // POST: ToursController/Edit/5
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [FromForm] CuriositiesPutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            multiContent.Add(new StringContent(model.Name), nameof(model.Name));
            if (model.NewImage != null)
            {
                var filesStreamContent = new StreamContent(model.NewImage.OpenReadStream());
                multiContent.Add(filesStreamContent, nameof(model.Image), model.NewImage.FileName);
            }

            multiContent.Add(new StringContent(model.Coordinates ?? ""), nameof(model.Coordinates));
            multiContent.Add(new StringContent(model.Longitude ?? ""), nameof(model.Longitude));
            multiContent.Add(new StringContent(model.Latitude ?? ""), nameof(model.Latitude));
            multiContent.Add(new StringContent(model.Type ?? ""), nameof(model.Type));
            multiContent.Add(new StringContent(model.Period ?? ""), nameof(model.Period));
            multiContent.Add(new StringContent(model.Country ?? ""), nameof(model.Country));
            multiContent.Add(new StringContent(model.Region ?? ""), nameof(model.Region));
            multiContent.Add(new StringContent(model.Province ?? ""), nameof(model.Province));
            multiContent.Add(new StringContent(model.City ?? ""), nameof(model.City));
            multiContent.Add(new StringContent(model.Description ?? ""), nameof(model.Description));

            var response = await client.PutAsync(BaseUrl + $"api/Curiosities/{id}", multiContent);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(response.Content.ReadAsStringAsync().Result);
            }

            if (model.SelectedToursIds == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!model.SelectedToursIds.Any())
            {
                return RedirectToAction(nameof(Index));
                //model.SelectedToursIds = new List<int>();
            }

            var PostToursForCuriosityResponse = await _CuriosityServices.PostToursForCuriosity(id, model.SelectedToursIds, User);
            if (!PostToursForCuriosityResponse.IsSucces)
            {
                return BadRequest(PostToursForCuriosityResponse.Result);
            }

            return RedirectToAction(nameof(Index));
        }


        // POST: ToursController/Delete/5
        [HttpPost]
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
            HttpResponseMessage Res = await client.DeleteAsync(BaseUrl + "api/Curiosities/" + id);

            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the curiosity object  
                //var curiosity = JsonConvert.DeserializeObject<Curiosity>(response);
                return RedirectToAction(nameof(Index));
            }
            var error = await Res.Content.ReadAsStringAsync();
            return BadRequest(error);
        }
    }
}
