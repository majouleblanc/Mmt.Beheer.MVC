using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mmt.Beheer.MVC.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.Services
{
    public interface IAuthServices
    {
        Task<ServicesResponseManager> LogInUserAsync(string email, string password);
    }

    public class AuthServices : IAuthServices
    {
        private readonly IConfiguration _Configuration;

        public string BaseURl { get; private set; }

        public AuthServices(IConfiguration configuration)
        {
            _Configuration = configuration;
            BaseURl = _Configuration["BaseUrl"];
        }


        public async Task<ServicesResponseManager> LogInUserAsync(string email, string password)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();



            var multicontent = new MultipartFormDataContent();

            multicontent.Add(new StringContent(email), "Email");
            multicontent.Add(new StringContent(password), "Password");

            var response = await client.PostAsync(BaseURl + $"api/Auth/Login", multicontent);

            if (!response.IsSuccessStatusCode)
            {
                return new ServicesResponseManager() { IsSucces = false, Result = response.StatusCode.ToString() };
            }

            var responseAsString = await response.Content.ReadAsStringAsync();
            var servicesResponseManager = JsonConvert.DeserializeObject<ServicesResponseManager>(responseAsString);

            var isDirector = servicesResponseManager.Roles.Contains("DIRECTOR");
            var isSupervisor = servicesResponseManager.Roles.Contains("SUPERVISOR");

            if (!isDirector)
            {
                if (!isSupervisor)
                {
                    servicesResponseManager.IsSucces = false;
                }
            }

            return servicesResponseManager;
        }
    }
}

