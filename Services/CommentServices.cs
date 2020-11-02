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
    public interface ICommentServices
    {
        Task<Comment> GetCommentWith(int id, ClaimsPrincipal User);
    }

    public class CommentServices : ICommentServices
    {
        private readonly IConfiguration _Configuration;
        public string BaseUrl { get; private set; }

        public CommentServices(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public async Task<Comment> GetCommentWith(int id, ClaimsPrincipal User)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);
            var response = await client.GetAsync(BaseUrl + $"api/Comments/{id}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.Content.ReadAsStringAsync().Result);
            }

            var CommentAsString =await response.Content.ReadAsStringAsync();
            var comment = JsonConvert.DeserializeObject<Comment>(CommentAsString);

            return comment;
        }
    }
}
