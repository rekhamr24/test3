using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SearchService.Interface;
using SearchService.Models;
using System.Net.Http;
using System.Configuration;
using System.Text.Json;


namespace SearchService.ExternalService
{
  

    public class GIFService : IGIFService
    {       
        

        /// <summary>
        /// Making external service call and building the response
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>

        public SearchServiceResponse GetExternalResponse(string searchTerm,string gifHostUrl,string apiKey,string limit)
        {
            var client = new HttpClient();
            string url = gifHostUrl + "api_key=" + apiKey + "&q=" + searchTerm + "&limit=" + limit;
            var lstSearchResponse = new List<SearchServiceResponseItem>();

            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var result_data = JsonSerializer.Deserialize<GIFServiceResponse>(result);

                List<Datum> obj1 = null;
                SearchServiceResponseItem _responseItem = new SearchServiceResponseItem();

                //Handling the requirment- Return exactly 5 if there 5 or more results available.

                if (result_data.data.Count >= 5)
                {
                    obj1 = result_data.data.Take(5).ToList();

                    foreach (var obj in obj1)
                    {
                        var url1 = obj.url;
                        var gif_id = obj.id;
                        var item = new SearchServiceResponseItem();
                        item.url = url1;
                        item.gif_id = gif_id;
                        lstSearchResponse.Add(item);
                    }

                }
                else
                {
                    //Handling the requirment- Return 0 results if there are less than 5 results available.
                    var item = new SearchServiceResponseItem();
                    item.url = string.Empty;
                    item.gif_id = string.Empty;
                    lstSearchResponse.Add(item);
                }
            }
            else
            {
                Console.WriteLine("GetExternalResponse returned exception" + response.StatusCode);

                var item = new SearchServiceResponseItem();
                item.url = string.Empty;
                item.gif_id = string.Empty;
                lstSearchResponse.Add(item);
            }

            var finalresponse = new SearchServiceResponse
            {
                data = lstSearchResponse
            };


            return finalresponse;
        }
    }
}
