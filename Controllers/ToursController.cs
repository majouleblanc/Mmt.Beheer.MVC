using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Mmt.Beheer.MVC.Extensions;
using Mmt.Beheer.MVC.Models;
using Mmt.Beheer.MVC.Services;
using Mmt.Beheer.MVC.ViewModels.Tours;
using Newtonsoft.Json;

namespace Mmt.Beheer.MVC.Controllers
{
    [Route("[Controller]/[Action]")]
    public class ToursController : Controller
    {
        private readonly IConfiguration _Configuration;
        private readonly IMapper _Mapper;
        private readonly ITourServices _TourServices;
        private readonly ICuriosityServices _CuriosityServices;

        private string BaseUrl { get; }

        public ToursController(IConfiguration configuration, IMapper mapper, ITourServices tourServices, ICuriosityServices curiosityServices)
        {
            _Configuration = configuration;
            _Mapper = mapper;
            _TourServices = tourServices;
            _CuriosityServices = curiosityServices;
            BaseUrl = _Configuration["BaseUrl"];
        }

        [HttpGet]
        // GET: ToursController
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();

            //Passing service base url  
            //client.BaseAddress = new Uri(BaseUrl);

            client.DefaultRequestHeaders.Clear();
            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.AddTokens(User);

            //Sending request to find web api REST service  using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/Tours");
            //Checking the response is successful or not which is sent using HttpClient  
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the tours list  
                var tours = JsonConvert.DeserializeObject<List<Tour>>(response);

                foreach (var tour in tours)
                {
                    tour.Curiosities = await _TourServices.GetCuriositiesForTour(tour.Id, User);
                }

                return View(tours);

            }
            //returning the tours list to view  
            return View();

        }


        // GET: ToursController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            using var client = new HttpClient();

            //Passing service base url  
            //client.BaseAddress = new Uri(BaseUrl);

            client.DefaultRequestHeaders.Clear();
            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.AddTokens(User);


            //Sending request to find web api REST service resource  using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/Tours/" + id);
            //Checking the response is successful or not which is sent using HttpClient  
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the tours list  
                var tour = JsonConvert.DeserializeObject<Tour>(response);
                tour.Curiosities = await _TourServices.GetCuriositiesForTour(tour.Id, User);
                return View(tour);
            }
            return View();
        }

        // GET: ToursController/Create
        public async Task<ActionResult> Create()
        {
            var model = new ToursPostViewModel();
            var Curiosities = await _CuriosityServices.GetAllCuriosities(User);

            model.CuriositiesSelectList = Curiosities.Any() ? new SelectList(Curiosities, "Id", "Name") : new SelectList("", "Id", "Name");

            //model.CuriositiesSelectList = new SelectList(Curiosities, "Id", "Name");
            //model.Curiosities = await _CuriosityServices.GetAllCuriosities();
            return View(model);
        }

        // POST: ToursController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] ToursPostViewModel model /* IFormCollection collection*/)
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
            multiContent.Add(new StringContent(model.Country ?? ""), nameof(model.Country));
            multiContent.Add(new StringContent(model.Region ?? ""), nameof(model.Region));
            multiContent.Add(new StringContent(model.Province ?? ""), nameof(model.Province));
            multiContent.Add(new StringContent(model.City ?? ""), nameof(model.City));
            multiContent.Add(new StringContent(model.Description ?? ""), nameof(model.Description));

            // send the post request to api to create the tour
            var response = await client.PostAsync(BaseUrl + "api/Tours/", multiContent);

            //if something went wrond displying the error in the screen
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(response.Content.ReadAsStringAsync().Result);
            }

            // if request succeeded returning reading the value and converting it to Tour object
            string retValue = await response.Content.ReadAsStringAsync();
            var tour = JsonConvert.DeserializeObject<Tour>(retValue);


            if (model.SelectedCuriositiesIds == null)
            {
                return RedirectToAction(nameof(Index));
            }

            //checking if the user has selected some curiosities 
            if (!model.SelectedCuriositiesIds.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            //send a post request to the api to add the curiosities for the tour
            var PostCuriositiesForTourResponse = await _TourServices.PostCuriositiesForTour(tour.Id, model.SelectedCuriositiesIds, User);
            if (!PostCuriositiesForTourResponse.IsSucces)
            {
                return BadRequest(PostCuriositiesForTourResponse.Result);
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
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.AddTokens(User);


            HttpResponseMessage resp = await client.GetAsync(BaseUrl + "api/Tours/" + id);
            if (resp.IsSuccessStatusCode)
            {
                var response = await resp.Content.ReadAsStringAsync();

                var tour = JsonConvert.DeserializeObject<Tour>(response);
                ToursPutViewModel model = _Mapper.Map<ToursPutViewModel>(tour);

                var curiosities = await _CuriosityServices.GetAllCuriosities(User);
                var tourCuriosities = await _TourServices.GetCuriositiesForTour(id, User);
                model.SelectedCuriositiesIds = tourCuriosities.Select(c => c.Id).ToList();

                model.CuriositiesSelectList = new SelectList(curiosities, "Id", "Name", model.SelectedCuriositiesIds);
                ViewData["id"] = id;
                return View(model);
            }
            return View();
        }

        // POST: ToursController/Edit/5
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [FromForm] ToursPutViewModel model)
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
            multiContent.Add(new StringContent(model.Country ?? ""), nameof(model.Country));
            multiContent.Add(new StringContent(model.Region ?? ""), nameof(model.Region));
            multiContent.Add(new StringContent(model.Province ?? ""), nameof(model.Province));
            multiContent.Add(new StringContent(model.City ?? ""), nameof(model.City));
            multiContent.Add(new StringContent(model.Description ?? ""), nameof(model.Description));

            var response = await client.PutAsync(BaseUrl + $"api/Tours/{id}", multiContent);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(response.Content.ReadAsStringAsync().Result);
            }

            //if there is no selected curiosities then we send an empty list so the curiosities for this tour get updated correctly
            if (model.SelectedCuriositiesIds == null || !model.SelectedCuriositiesIds.Any())
            {
                model.SelectedCuriositiesIds = new List<int>();
            }

            if (model.SelectedCuriositiesIds.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            //there is some curiosities selected we send a post request to api
            var PostCuriositiesForTourResponse = await _TourServices.PostCuriositiesForTour(id, model.SelectedCuriositiesIds, User);
            if (!PostCuriositiesForTourResponse.IsSucces)
            {
                return BadRequest(PostCuriositiesForTourResponse.Result);
            }

            return RedirectToAction(nameof(Index));

            //var retValue = await response.Content.ReadAsStreamAsync();
            //return View(model);
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
            HttpResponseMessage Res = await client.DeleteAsync(BaseUrl + "api/Tours/" + id);

            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the tours list  
                var tour = JsonConvert.DeserializeObject<Tour>(response);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
