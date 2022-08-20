using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Binke.Utility
{
    public static class APIHelper
    {
        public static string BaseAddress
        {
            get
            {
                string _apiBaseAddress = "";
                if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ApiBaseAddress"]))
                {
                    _apiBaseAddress = ConfigurationManager.AppSettings["ApiBaseAddress"].ToString();
                }
                return _apiBaseAddress;
            }
        }

        public async static Task<T> GetAsync<T>(string apiKey /*controller/action*/) where T : class
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(BaseAddress + apiKey);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data);
            }
            else
            {
                return null;
            }
        }

        public async static Task<T> PostAsync<T>(string apiKey, object parameterModel) where T : class
        {
            HttpClient client = new HttpClient();
            string jsonObject = JsonConvert.SerializeObject(parameterModel, Formatting.Indented);
            HttpContent content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(BaseAddress + apiKey, content);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data);
            }
            else
            {
                return null;
            }
        }
    }
}
