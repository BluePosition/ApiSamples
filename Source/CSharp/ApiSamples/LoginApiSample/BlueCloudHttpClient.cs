using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BluePosition.Samples.LoginApi
{
    public static class HttpClientHelpers
    {
        public const string JsonContentType = "application/json";

        public static HttpClient Create(string ocpApimSubscriptionKey)
        {
            var client = new HttpClient();
            // This header is required to authorize the request
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ocpApimSubscriptionKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonContentType));

            return client;
        }

        public static HttpClient CreateWithToken(string ocpApimSubscriptionKey, string token)
        {
            var client = Create(ocpApimSubscriptionKey);
            client.DefaultRequestHeaders.Add("x-tdc-token", token);

            return client;
        }
    }
}
