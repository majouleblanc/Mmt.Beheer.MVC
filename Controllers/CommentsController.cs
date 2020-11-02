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
using Mmt.Beheer.MVC.ViewModels.Comment;
using Newtonsoft.Json;

namespace Mmt.Beheer.MVC.Controllers
{
    [Route("[Controller]/[Action]")]
    public class CommentsController : Controller
    {
        private readonly IConfiguration _Configuration;
        private readonly IMapper _Mapper;
        private readonly ICuriosityServices _CuriosityServices;

        public string BaseUrl { get; }

        public CommentsController(IConfiguration configuration, IMapper mapper, ICuriosityServices curiosityServices)
        {
            _Configuration = configuration;
            _Mapper = mapper;
            _CuriosityServices = curiosityServices;

            BaseUrl = _Configuration["BaseUrl"];
        }

        // GET: CommentsController
        [Route("{curiosityId}")]
        public async Task<ActionResult> Index(int curiosityId)
        {
            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource GetAllCuriosities using HttpClient  
            HttpResponseMessage Res;
            if (curiosityId <= 0)
            {
                return BadRequest($"invalid curiosity id, ID {curiosityId}");
                //Res = await client.GetAsync(BaseUrl + "api/Comments");
            }

            var curiosity = await _CuriosityServices.GetCuriosity(curiosityId, User);

            if (curiosity == null)
            {
                return NotFound($"curiosity with id = {curiosityId} not found");
            }

            Res = await client.GetAsync(BaseUrl + "api/Comments" + $"/CuriosityComments/{curiosityId}");


            //Checking the response is successful or not which is sent using HttpClient  
            if (!Res.IsSuccessStatusCode)
            {
                return BadRequest($"cant fetch the comment list from api");
            }
            //Storing the response details recieved from web api   
            var response = await Res.Content.ReadAsStringAsync();

            //Deserializing the response recieved from web api and storing into the Curiosity list  
            var Comments = JsonConvert.DeserializeObject<List<Comment>>(response);
            ViewData["curiosityId"] = curiosityId;
            //returning the curiosity list to view  
            return View(Comments);
        }

        // GET: CommentsController/Details/5
        public async Task<ActionResult> Details(int id, int curiosityId)
        {
            if (curiosityId <= 0)
            {
                return BadRequest($"invalid curiosity id, Id : {curiosityId}");
            }

            var curiosity = await _CuriosityServices.GetCuriosity(curiosityId, User);
            if (curiosity == null)
            {
                return NotFound($"curiosity with id :{curiosity} not found");
            }

            if (id <= 0)
            {
                return BadRequest($"invalid photo id, Id : {id}");
            }


            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource GetAllCuriosities using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/Comments/" + id);
            //Checking the response is successful or not which is sent using HttpClient  
            if (!Res.IsSuccessStatusCode)
            {
                return BadRequest($"could not fetch data from api");
            }
            //Storing the response details recieved from web api   
            var response = await Res.Content.ReadAsStringAsync();

            //Deserializing the response recieved from web api and storing into the Curiosity list  
            var comment = JsonConvert.DeserializeObject<Comment>(response);
            return View(comment);
        }

        // GET: CommentsController/Create
        [Route("{curiosityId}")]
        public async Task<ActionResult> Create(int curiosityId)
        {
            if (curiosityId <= 0)
            {
                return BadRequest($"invalid curiosity id, Id : {curiosityId}");
            }

            var curiosity = await _CuriosityServices.GetCuriosity(curiosityId, User);
            if (curiosity == null)
            {
                return NotFound($"curiosity with id :{curiosity} not found");
            }
            var model = new CommentPostViewModel() { CuriosityId = curiosityId };
            return View(model);
        }

        // POST: CommentsController/Create
        [HttpPost("{curiosityId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int curiosityId, [FromForm] CommentPostViewModel model/*IFormCollection collection*/)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "data not valid");
                return View();
            }

            if (curiosityId <= 0)
            {
                return BadRequest($"curiosity id not valid, curiosityId : {curiosityId}");
            }

            var curiosity = await _CuriosityServices.GetCuriosity(curiosityId, User);
            if (curiosity == null)
            {
                return NotFound($"curiosity with id : {curiosity} not found");
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);


            var multiContent = new MultipartFormDataContent();

            multiContent.Add(new StringContent(curiosityId.ToString()), nameof(model.CuriosityId));
            multiContent.Add(new StringContent(model.UserName), nameof(model.UserName));
            multiContent.Add(new StringContent(model.CommentMsg ?? ""), nameof(model.CommentMsg));
            multiContent.Add(new StringContent(model.Email ?? ""), nameof(model.Email));


            var response = await client.PostAsync(BaseUrl + "api/Comments/", multiContent);
            if (!response.IsSuccessStatusCode)
            {
                BadRequest(response.StatusCode);
            }

            var retValue = await response.Content.ReadAsStreamAsync();
            return RedirectToAction(nameof(Index), new { curiosityId = curiosityId });
        }

        // GET: CommentsController/Edit/5
        [HttpGet("{curiosityId}/{id}")]
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

            HttpResponseMessage resp = await client.GetAsync(BaseUrl + "api/Comments/" + id);
            if (resp.IsSuccessStatusCode)
            {
                var response = await resp.Content.ReadAsStringAsync();

                var comment = JsonConvert.DeserializeObject<Comment>(response);
                var model = _Mapper.Map<CommentPutViewModel>(comment);
                ViewData["id"] = id;
                return View(model);
            }
            return View();
        }

        // POST: CommentsController/Edit/5
        [HttpPost("{curiosityId}/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int curiosityId, int id, [FromForm] CommentPutViewModel model)
        {
            if (!ModelState.IsValid || id <= 0)
            {
                return View(model);
            }

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(new StringContent(id.ToString()), "id");
            multiContent.Add(new StringContent(curiosityId.ToString()), nameof(model.CuriosityId));
            multiContent.Add(new StringContent(model.UserName), nameof(model.UserName));
            multiContent.Add(new StringContent(model.CommentMsg ?? ""), nameof(model.CommentMsg));
            multiContent.Add(new StringContent(model.Email ?? ""), nameof(model.Email));

            var response = await client.PutAsync(BaseUrl + $"api/Comments/{id}", multiContent);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"could not update the comment in the api");
            }

            return RedirectToAction(nameof(Index), new { curiosityId = curiosityId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("", $"Tour Id not valid");
                return View();
            }

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //HTTP DELETE
            HttpResponseMessage Res = await client.DeleteAsync(BaseUrl + "api/Comments/" + id);

            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the Curiosity list  
                var comment = JsonConvert.DeserializeObject<Comment>(response);
                return RedirectToAction(nameof(Index), new { curiosityId = comment.CuriosityId });
            }
            return View();
        }
    }
}
