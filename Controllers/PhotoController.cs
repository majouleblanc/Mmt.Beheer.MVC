using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mmt.Beheer.MVC.Models;
using Mmt.Beheer.MVC.Services;
using Mmt.Beheer.MVC.ViewModels.Photo;

namespace Mmt.Beheer.MVC.Controllers
{
    [Route("[Controller]/[Action]")]
    public class PhotoController : Controller
    {
        private readonly IConfiguration _Configuration;
        private readonly IPhotoServices _PhotoServices;
        private readonly ICuriosityServices _CuriosityServices;

        public string BaseUrl { get; private set; }

        public PhotoController(IConfiguration configuration, IPhotoServices photoServices, ICuriosityServices curiosityServices)
        {
            _Configuration = configuration;
            _PhotoServices = photoServices;
            _CuriosityServices = curiosityServices;
            BaseUrl = _Configuration["BaseUrl"];
        }

        // GET: PhotoController
        [Route("{curiosityId}")]
        public async Task<ActionResult> Index(int curiosityId)
        {
            if (curiosityId <= 0)
            {
                return BadRequest($"invalid curiosityId, ID : {curiosityId} ");
            }

            var curiosity = await _CuriosityServices.GetCuriosity(curiosityId, User);
            if (curiosity == null)
            {
                return NotFound($"curiosity with Id : {curiosityId} not found");
            }

            //var photos = new List<Photo> ();
            //return View(photos);

            var model = await _PhotoServices.GetPhotoForCuriosityWith(curiosityId, User);
            ViewData["curiosityId"] = curiosityId;
            return View(model);
        }

        // GET: PhotoController/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        // GET: PhotoController/Create
        [Route("{curiosityId}")]
        public ActionResult Create(int curiosityId)
        {
            var model = new PhotoPostViewModel() { CuriosityId = curiosityId };
            return View(model);
        }

        // POST: PhotoController/Create
        [HttpPost("{curiosityId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int curiosityId,[FromForm] PhotoPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Form data not valid");
                return View(ModelState);
            }
            if (curiosityId <= 0)
            {
                return BadRequest($"invalid curiosityId, ID : {curiosityId} ");
            }

            //if (model.Photo == null)
            //{
            //    return BadRequest($"photo cannot be null");
            //}

            var curiosity = await _CuriosityServices.GetCuriosity(curiosityId, User);
            if (curiosity == null)
            {
                return NotFound($"curiosity with Id : {curiosityId} not found");
            }

            var photo = await _PhotoServices.PostPhotoForCuriosityWith(curiosityId, model.Photo, User);

            return RedirectToAction(nameof(Index), new { curiosityId = curiosityId });
        }


        // GET: PhotoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PhotoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: PhotoController/Delete/5
        [HttpPost("{curiosityId}/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int curiosityId, int id)
        {
            if (id <=0)
            {
                return BadRequest($"invalide photo id");
            }

            var photo = await _PhotoServices.DeletePhoto(id, User);

            if (photo == null)
            {
                NotFound($"photo with id : {id} not found");
            }

            return RedirectToAction(nameof(Index), new { curiosityId = curiosityId });

        }
    }
}
