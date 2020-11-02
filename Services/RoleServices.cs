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
    public interface IRoleServices
    {
        Task<List<Role>> GetRolesForUserWith(string userId, ClaimsPrincipal User);
        Task<List<Role>> GetAllRoles(ClaimsPrincipal User);
        Task<List<MmtUser>> GetUsersInRole(string id, ClaimsPrincipal User);
    }
    public class RoleServices : IRoleServices
    {
        private readonly IConfiguration _Configuration;
        public string BaseUrl { get; private set; }

        public RoleServices(IConfiguration configuration)
        {
            _Configuration = configuration;
            BaseUrl = _Configuration["BaseUrl"];
        }
        public async Task<List<Role>> GetRolesForUserWith(string userId, ClaimsPrincipal User)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            var response =await client.GetAsync(BaseUrl + $"api/Administration/userRoles/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.Content.ReadAsStringAsync().Result);
            }

            var rolesAsString = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject <List<Role>>(rolesAsString);

            return roles;
        }
        public async Task<List<Role>> GetAllRoles(ClaimsPrincipal User)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            var response = await client.GetAsync(BaseUrl + $"api/Roles");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.Content.ReadAsStringAsync().Result);
            }

            var rolesAsString = await response.Content.ReadAsStringAsync();

            var roles = JsonConvert.DeserializeObject<List<Role>>(rolesAsString);

            return roles;
        }

        public async Task<List<MmtUser>> GetUsersInRole(string roleId, ClaimsPrincipal User)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.AddTokens(User);

            var response = await client.GetAsync(BaseUrl + $"api/Administration/GetUsersInRole/{roleId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.Content.ReadAsStringAsync().Result);
            }

            var usersInRoleAsString = await response.Content.ReadAsStringAsync();
            var usersInRole = JsonConvert.DeserializeObject<List<MmtUser>>(usersInRoleAsString);
            return usersInRole;
        }
    }
}
