using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Extensions.Configuration;
using Mmt.Beheer.MVC.Extensions;
using Mmt.Beheer.MVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.Services
{
    public interface IPhotoServices
    {
        Task<List<Photo>> GetPhotoForCuriosityWith(int curiosityId, ClaimsPrincipal User);
        Task<Photo> GetPhotoWith(int id, ClaimsPrincipal User);
        Task<Photo> PostPhotoForCuriosityWith(int curiosityId, IFormFile photo, ClaimsPrincipal User);
        Task<Photo> DeletePhoto(int id, ClaimsPrincipal User);
    }
    public class PhotoServices : IPhotoServices
    {
        private readonly IConfiguration _Configuration;
        public string BaseUrl { get; set; }

        //ctor
        public PhotoServices(IConfiguration configuration)
        {
            _Configuration = configuration;
            BaseUrl = _Configuration["BaseUrl"];
        }


        public async Task<List<Photo>> GetPhotoForCuriosityWith(int curiosityId, ClaimsPrincipal User)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            var response =  await client.GetAsync(BaseUrl + $"api/Photos/CuriosityWithId/{curiosityId}");

            if (!response.IsSuccessStatusCode)
            {
                new Exception(response.Content.ReadAsStringAsync().Result);
            }

            var PhotoListAsString = await response.Content.ReadAsStringAsync();

            var photoList = JsonConvert.DeserializeObject<List<Photo>>(PhotoListAsString);

            return photoList;
        }


        public async Task<Photo> PostPhotoForCuriosityWith(int curiosityId, IFormFile photo, ClaimsPrincipal User)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            var multicontent = new MultipartFormDataContent();
            multicontent.Add(new StringContent(curiosityId.ToString()), nameof(curiosityId));

            var fileStreamContent = new StreamContent(photo.OpenReadStream());

            multicontent.Add(fileStreamContent, "Photo", photo.FileName);

            var response = await client.PostAsync(BaseUrl + "api/Photos/", multicontent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.Content.ReadAsStringAsync().Result);
            }

            var photoAsString =await response.Content.ReadAsStringAsync();
            var photoToReturn = JsonConvert.DeserializeObject<Photo>(photoAsString);

            return photoToReturn;

        }


        public async Task<Photo> DeletePhoto(int id, ClaimsPrincipal User)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            var response = await client.DeleteAsync(BaseUrl + $"api/Photos/{id}");

            var photoAsString =await response.Content.ReadAsStringAsync();

            var photo = JsonConvert.DeserializeObject<Photo>(photoAsString);
            return photo;
        }

        public async Task<Photo> GetPhotoWith(int id, ClaimsPrincipal User)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            var response = await client.GetAsync(BaseUrl + $"api/Photos/{id}");

            var photoAsString = await response.Content.ReadAsStringAsync();

            var photo = JsonConvert.DeserializeObject<Photo>(photoAsString);

            return photo;
            
        }
    }
}
