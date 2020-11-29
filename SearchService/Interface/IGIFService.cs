using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SearchService.Models;

namespace SearchService.Interface
{
    interface IGIFService
    {
        SearchServiceResponse GetExternalResponse(string searchTerm,string gifHostUrl, string apiKey, string limit);
    }
}
