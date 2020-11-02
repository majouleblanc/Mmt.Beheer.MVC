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
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.Services
{
    public interface ITourServices
    {
        Task<List<Curiosity>> GetCuriositiesForTour(int tourId, ClaimsPrincipal User);
        Task<ServicesResponseManager> PostCuriositiesForTour(int tourId, List<int> selectedCuriositiesIds, ClaimsPrincipal User);
        Task<List<Tour>> GetAllTours(ClaimsPrincipal User);

    }

    public class ToursServices : ITourServices
    {
        private readonly IConfiguration _Configuration;

        public string BaseUrl { get; private set; }

        public ToursServices(IConfiguration configuration)
        {
            _Configuration = configuration;
            BaseUrl = _Configuration["BaseUrl"];
        }
        public async Task<List<Tour>> GetAllTours(ClaimsPrincipal User)
        {
            using var client = new HttpClient();
            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource Curiosities using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/Tours");
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

        // Get curiosities for a specific tour
        public async Task<List<Curiosity>> GetCuriositiesForTour(int tourId, ClaimsPrincipal User)
        {
            using var client = new HttpClient();

            //Passing service base url  

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Sending request to find web api REST service resource Curiosities using HttpClient  
            HttpResponseMessage Res = await client.GetAsync(BaseUrl + "api/ToursAndCuriositiesManager/GetTourCuriosities/" + tourId);
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

        // sets curiosities for a specific tour
        public async Task<ServicesResponseManager> PostCuriositiesForTour(int tourId, List<int> selectedCuriositiesIds, ClaimsPrincipal User)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);


            var multiContent = new MultipartFormDataContent();

            if (selectedCuriositiesIds.Count == 0)
            {
                multiContent.Add(new StringContent(""), "CuriositiesIds");
            }

            foreach (var id in selectedCuriositiesIds)
            {
                multiContent.Add(new StringContent(id.ToString()), "CuriositiesIds");
            }

            var response = await client.PostAsync(BaseUrl + $"api/ToursAndCuriositiesManager/ManageCuriositiesForTourWith/{tourId}/", multiContent);
            if (response.IsSuccessStatusCode)
            {
                //var retValue =  await response.Content.ReadAsStreamAsync();
                var retValue =  await response.Content.ReadAsStringAsync();
                return new ServicesResponseManager() { IsSucces = true, Result = retValue };
            }
            else
            {
                return new ServicesResponseManager() { IsSucces = false, Result = response.Content.ReadAsStringAsync().Result };
            }
        }

    }
}
