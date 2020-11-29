using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SearchService.Models;
using SearchService.ExternalService;
using System.Net.Http;
using System.Net;
using System.Text;
using Newtonsoft;


namespace SearchService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private IConfiguration Configuration;

        public SearchController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }       
      
        GIFService _service = new GIFService();
        HttpRequestMessage Request=new HttpRequestMessage();

        [HttpGet("{id}")]     
      public IActionResult Get(string id)
        {
            var serviceResponse = new SearchServiceResponse();
            try
            {
                string searchTerm = id;
                
                string gifHostUrl = Configuration.GetSection("APISettings").GetSection("GifHostUrl").Value;
                string apiKey = Configuration.GetSection("APISettings").GetSection("ApiKey").Value;
                string limit = Configuration.GetSection("APISettings").GetSection("Limit").Value;

                //call the gif external service
                serviceResponse = _service.GetExternalResponse(searchTerm, gifHostUrl, apiKey, limit);


                //response data count > 1, the transaction is successful
                if (serviceResponse.data.Count > 1)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    var responseString = Newtonsoft.Json.JsonConvert.SerializeObject(serviceResponse);                    
                    var content= new StringContent(responseString, Encoding.UTF8, "application/json");                  

                    return new ContentResult()
                    {
                        Content = responseString,
                        ContentType = "application/json",
                        StatusCode = 200
                    };
                }
                else
                {
                    //exception occured during the gif external service call and process
                    var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                    var responseString = Newtonsoft.Json.JsonConvert.SerializeObject(serviceResponse);
                    response.Content = new StringContent(responseString, Encoding.UTF8, "application/json");                   
                    return new ContentResult()
                    {
                        Content = responseString,
                        ContentType = "application/json",
                        StatusCode = 204
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception from Get method" + ex.Message);
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                var responseString = Newtonsoft.Json.JsonConvert.SerializeObject(serviceResponse);
                response.Content = new StringContent(responseString, Encoding.UTF8, "application/json");               
                return new ContentResult()
                {
                    Content = responseString,
                    ContentType = "application/json",
                    StatusCode=500
                };

            }

        }
    }
}