using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public class PayUApiClient
    {
        private readonly HttpClient _httpClient;

        public PayUApiClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> MakePayment()
        {
            try
            {
                var url = "https://test.payu.in/merchant/_payment";
                var parameters = new Dictionary<string, string>
            {
                { "key", "JP***g" },
                { "amount", "10.00" },
                { "txnid", "txnid87267147744" },
                { "firstname", "PayU User" },
                { "email", "test@gmail.com" },
                { "phone", "9876543210" },
                { "productinfo", "iPhone" },
                { "surl", "https://test-payment-middleware.payu.in/simulatorResponse" },
                { "furl", "https://test-payment-middleware.payu.in/simulatorResponse" },
                // Add other parameters as needed
            };

                var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"Failed with status code {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }
    }
}