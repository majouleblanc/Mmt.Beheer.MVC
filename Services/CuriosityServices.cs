using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mmt.Beheer.MVC.Extensions;
using Mmt.Beheer.MVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.Services
{
    public interface ICuriosityServices
    {
        Task<List<Curiosity>> GetAllCuriosities(ClaimsPrincipal User);
        Task<List<Tour>> GetToursForCuriosity(int curiosityId, ClaimsPrincipal User);
        Task<ServicesResponseManager> PostToursForCuriosity(int curiosityId, List<int> selectedToursIds, ClaimsPrincipal User);
        Task<CuriosityLike> GetCuriosityLike(int curiosityId, ClaimsPrincipal User);
        Task<List<Photo>> GetCuriostyPhotos(int curiosityId, ClaimsPrincipal User);
        Task<Curiosity> GetCuriosity(int curiosityId, ClaimsPrincipal User);
    }
    public class CuriosityServices : ICuriosityServices
    {
        public string BaseUrl { get; set; }
        private readonly IConfiguration _Configuration;

        public CuriosityServices(IConfiguration configuration)
        {
            _Configuration = configuration;
            BaseUrl = _Configuration["BaseUrl"];
        }

        public async Task<List<Curiosity>> GetAllCuriosities(ClaimsPrincipal User)
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
                //returning the curiosity list to view  
                return curiosities;

            }
            throw new Exception(Res.Content.ReadAsStringAsync().Result);
        }


        public async Task<List<Tour>> GetToursForCuriosity(int curiosityId, ClaimsPrincipal User)
        {
            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource Curiosities using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/ToursAndCuriositiesManager/GetCuriosityTours/" + curiosityId);
            //Checking the response is successful or not which is sent using HttpClient  
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the Curiosity list  
                var tours = JsonConvert.DeserializeObject<List<Tour>>(response);
                //returning the curiosity list to view  
                return tours;
            }

            throw new Exception(Res.Content.ReadAsStringAsync().Result);
        }

        public async Task<CuriosityLike> GetCuriosityLike(int curiosityId, ClaimsPrincipal User)
        {
            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource Curiosities using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/CuriosityLikes/" + curiosityId);
            //Checking the response is successful or not which is sent using HttpClient  
            if (!Res.IsSuccessStatusCode)
            {
                return new CuriosityLike() { CuriosityId = curiosityId, Likes = 0 };
            }
            //Storing the response details recieved from web api   
            var response = await Res.Content.ReadAsStringAsync();

            //Deserializing the response recieved from web api and storing into the Curiosity list  
            var curiosityLike = JsonConvert.DeserializeObject<CuriosityLike>(response);
            //returning the curiosity list to view  
            return curiosityLike;

            throw new Exception(Res.Content.ReadAsStringAsync().Result);
        }

        public async Task<ServicesResponseManager> PostToursForCuriosity(int curiosityId, List<int> selectedToursIds, ClaimsPrincipal User)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);


            var multiContent = new MultipartFormDataContent();

            foreach (int id in selectedToursIds)
            {
                multiContent.Add(new StringContent(id.ToString()), "toursIds");
            }

            var response = await client.PostAsync(BaseUrl + $"api/ToursAndCuriositiesManager/ManageToursForCuriosityWith/" + curiosityId, multiContent);
            if (response.IsSuccessStatusCode)
            {
                //var retValue = await response.Content.ReadAsStreamAsync();
                var retValue = await response.Content.ReadAsStringAsync();
                return new ServicesResponseManager() { IsSucces = true, Result = retValue };
            }
            else
            {
                return new ServicesResponseManager() { IsSucces = false, Result = response.Content.ReadAsStringAsync().Result };
            }
        }

        public async Task<List<Photo>> GetCuriostyPhotos(int curiosityId, ClaimsPrincipal User)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            var response = await client.GetAsync(BaseUrl + $"api/Photos/CuriosityWithId/{curiosityId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"{response.Content.ReadAsStringAsync().Result}");
            }

            var retValue = await response.Content.ReadAsStringAsync();

            var photos = JsonConvert.DeserializeObject<List<Photo>>(retValue);

            return photos;
        }

        public async Task<Curiosity> GetCuriosity(int curiosityId, ClaimsPrincipal User)
        {
            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource GetAllCuriosities using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/Curiosities/" + curiosityId);
            //Checking the response is successful or not which is sent using HttpClient  
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api   
                var response = await Res.Content.ReadAsStringAsync();

                //Deserializing the response recieved from web api and storing into the Curiosity list  
                var curiosity = JsonConvert.DeserializeObject<Curiosity>(response);
                return curiosity;
            }
            return null;
        }

    }
}