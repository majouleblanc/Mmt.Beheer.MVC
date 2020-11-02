using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mmt.Beheer.MVC.Extensions
{
    public static class HttpClientExtensions
    {
       public static void AddTokens(this HttpClient client, ClaimsPrincipal user)
        {
            var token = user.Claims.FirstOrDefault(c => c.Type == "token").Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }
    }
}
