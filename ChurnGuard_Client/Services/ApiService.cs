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
                    MessageBox.Show("모델별 성능 비교 결과를 가져왔습니다.");
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
                MessageBox.Show("테스트 결과를 가져왔습니다.");
                return await response.Content.ReadFromJsonAsync<List<ModelMetric>>()?? new();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new List<ModelMetric>();
            }
        }
        public async Task<FinalReportResponse?> LoadFinalReport()
        {
            // Python 서버의 GridSearchCV 엔드포인트 호출
            return await client.GetFromJsonAsync<FinalReportResponse>("/api/model-report");
        }
    }
}
