using RestSharp;

namespace Vastwebmulti.Areas.RETAILER.Controllers
{
    /// <summary>
    /// Utility class for making REST API calls to TBO (travel booking) endpoints.
    /// </summary>
    public class TBO_Utils
    {
        /// <summary>
        /// Sends a POST request to the specified URL with the given request data and returns the raw response content.
        /// </summary>
        /// <param name="requestData">The JSON-serialized request payload to send.</param>
        /// <param name="url">The target endpoint URL.</param>
        /// <returns>The raw response content string, or null if the request fails.</returns>
        public static string GetResponse(string requestData, string url)
        {
            try
            {
                //
                var client = new RestClient("http://localhost:59382/api/Air/Search");
                var request = new RestRequest(Method.POST);
                request.AddHeader("postman-token", "5500ffab-0e31-c9d2-6925-8e45009b0198");
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var respo = response.Content;
                return respo;
                //
                /*var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:55587/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //GET Method  
                // HttpResponseMessage response =  client.PostAsJsonAsync(url, requestData).Result;
                HttpResponseMessage response = client.PostAsJsonAsync(url, requestData).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseJS =  response.Content.ReadAsAsync<dynamic>();
                    return JsonConvert.SerializeObject(responseJS);
                }
                else
                {
                    return null;
                }*/
            }
            catch
            {
                return null;
            }
        }

    }
}