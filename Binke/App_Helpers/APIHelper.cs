using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Binke.Models;
using Doctyme.Model;
using Newtonsoft.Json;

namespace Binke.App_Helpers
{
    public static class APIHelper
    {
        public async static Task<T> GetAsyncById<T>(int id,string apiKey) where T:class
        {
            using (HttpClient client =new HttpClient())
            { 
                var baseAddress= System.Configuration.ConfigurationManager.AppSettings["ApiBaseAddress"];
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(apiKey+"?id="+id).ConfigureAwait(false);
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

        public async static Task<T> GetAsync<T>(string apiKey) where T : class
        {
            using (HttpClient client = new HttpClient())
            {
                var baseAddress = System.Configuration.ConfigurationManager.AppSettings["ApiBaseAddress"];
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(apiKey).ConfigureAwait(false);
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

        public async static Task<IEnumerable<T>> GetAsyncList<T>(string apiKey) where T : class
        {
            using (HttpClient client = new HttpClient())
            {
                var baseAddress = System.Configuration.ConfigurationManager.AppSettings["ApiBaseAddress"];
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear(); 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(apiKey).ConfigureAwait(false);
               
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<T>>(data);
                }
                else
                {
                    return null;
                }
            }
        }

        public async static Task<IEnumerable<T>> GetAsyncListById<T>(int id,string apiKey) where T : class
        {
            using (HttpClient client = new HttpClient())
            {
                var baseAddress = System.Configuration.ConfigurationManager.AppSettings["ApiBaseAddress"];
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                HttpResponseMessage response = await client.GetAsync(apiKey+"?id="+id).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<T>>(data);
                }
                else
                {
                    return null;
                }
            }
        }

        public async static Task<T> PostAsync<T>(T obj,string apiKey) where T : class
        {
            using (HttpClient client = new HttpClient())
            {
                var baseAddress = System.Configuration.ConfigurationManager.AppSettings["ApiBaseAddress"];
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string data = JsonConvert.SerializeObject(obj);
                StringContent stringContent = new StringContent(data);
                HttpResponseMessage response = await client.PostAsync(apiKey, stringContent).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var returnResult = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(returnResult);
                }
                else
                {
                    return null;
                }
            }
        }


        // Synchronous method


        public static T GetSync<T>(string apiKey) where T : class
        {
            using (HttpClient client = new HttpClient())
            {
                var baseAddress = System.Configuration.ConfigurationManager.AppSettings["ApiBaseAddress"];
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //HTTP GET
                var response = client.GetAsync(apiKey);
                response.Wait();
                //Get Result
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var data = result.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(data.Result);
                }
                else
                {
                    return null;
                }
            }
        }

        public  static IEnumerable<T> GetSyncList<T>(string apiKey) where T : class
        {
            using (HttpClient client = new HttpClient())
            {
                var baseAddress = System.Configuration.ConfigurationManager.AppSettings["ApiBaseAddress"];
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //HTTP GET
                
                var response= client.GetAsync(apiKey);
                response.Wait();
                //Get Result
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var data = result.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<T>>(data.Result);
                }
                else
                {
                    return null;
                }
            }
        }
        public static List<T> GetSyncListPagination<T>(Pagination pager, string apiKey) where T : class
        {
            using (HttpClient client = new HttpClient())
            {
                var baseAddress = System.Configuration.ConfigurationManager.AppSettings["ApiBaseAddress"];
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //HTTP GET
                string dataobj = JsonConvert.SerializeObject(pager);
                StringContent stringContent = new StringContent(dataobj);
                var response = client.GetAsync(apiKey+"?StartIndex="+ pager.StartIndex+"&PageSize="+pager.PageSize+"&Search="+pager.Search + "&SortColumnName=" + pager.SortColumnName + "&SortDirection=" + pager.SortDirection);
                response.Wait();
                //Get Result
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var data = result.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<T>>(data.Result);
                }
                else
                {
                    return new List<T>();
                }
            }
        }

        // End

        #region API URL
        public static string GetDoctorDetailsById="api/DashBoard/GetDoctorDetailsById";


        // Added By Nagaraj 13-jan-2020
        public static string GetFeaturedSpecialityDetails = "api/Home/GetFeaturedSpecialityDetails";
        public static string GetFeaturedDoctorsDetails = "api/Home/GetFeaturedDoctorsDetails";
        public static string GetDoctorsFacilityDetails = "api/Home/GetDoctorsFacilityDetails";
        public static string GetDoctorCount = "api/Home/GetDoctorCount";
        public static string GetFacilityCount = "api/Home/GetFacilityCount";
        public static string GetPharmacyCount = "api/Home/GetPharmacyCount";
        public static string GetSeniorCareCount = "api/Home/GetSeniorCareCount";
        // End

        public static string GetHomePageFeaturedDoctorList = "api/Home/GetHomePageFeaturedDoctorList";


        public static string GetHomePageFeaturedFacilityList = "api/Home/GetHomePageFeaturedFacilityList";

        #endregion
        #region DoctorApi
        //Added By Shubham
        public static string GetAllDoctorsList = "api/Doctor/GetAllDoctors";
        #endregion
    }
}
