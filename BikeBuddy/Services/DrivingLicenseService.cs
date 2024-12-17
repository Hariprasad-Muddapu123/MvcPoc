using System.Net.Http.Headers;

namespace BikeBuddy.Services
{
    public class DrivingLicenseService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.example.com/verify-license?licenseNumber={0}&apiKey=YOUR_API_KEY";

        public DrivingLicenseService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> GetLicenseDetailsAsync(string licenseNumber)
        {
            var requestUrl = string.Format(ApiUrl, licenseNumber);

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
