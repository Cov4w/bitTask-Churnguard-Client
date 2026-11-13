using ChurnGuard_Client.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace ChurnGuard_Client.Services
{
    internal class ApiService
    {
        private readonly HttpClient client;

        public ApiService()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8000")
            };
        }
        public async Task<List<ModelMetric>> LoadBasicPerformance()
        {
            try
            {
                List<ModelMetric>? results = await client.GetFromJsonAsync<List<ModelMetric>>("/api/model-performance");
                if (results != null)
                {
                    return results;
                }
                return new List<ModelMetric>();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new List<ModelMetric>();
            }
        }
        public async Task<List<ModelMetric>> TestImprovement(TestFigure figure)
        {
            try
            {
                var response = await client.PostAsJsonAsync("/api/model-improvement", figure);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ModelMetric>>()?? new();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new List<ModelMetric>();
            }
        }
    }
}
