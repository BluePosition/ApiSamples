using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BluePosition.Samples.LoginApi
{
    public class LoginTask
    {
        private readonly string ocpApimSubscriptionKey;
        public LoginTask(string ocpApimSubscriptionKey)
        {
            this.ocpApimSubscriptionKey = ocpApimSubscriptionKey;
        }

        public async Task<string> Execute(string username, string password)
        {
            const string url = "https://api.cloud.mobileservices.dk/login/api/login";

            // The login operation is a POST method taken a JSON body with the user name/password
            var jsonUser = $"{{'Username': '{username}', 'Password': '{password}'}}";
            var content = new StringContent(jsonUser, Encoding.UTF8, HttpClientHelpers.JsonContentType);

            // Posting the request
            var response = await HttpClientHelpers.Create(ocpApimSubscriptionKey).PostAsync(url, content);

            var token = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                var definition = new { Token = "" };
                token = JsonConvert.DeserializeAnonymousType(tokenResponse, definition).Token;
            }
            else
            {
                Console.WriteLine($"Unable to log in using the provided credentials. The HTTP Status Code is: {response.StatusCode}.");
            }

            return token;
        }
    }
}
