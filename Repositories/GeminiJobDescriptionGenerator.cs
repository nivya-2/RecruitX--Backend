using System.Net.Http;
using System.Text;
using System.Text.Json;
using RecruitX.Interfaces;

namespace RecruitX.AI
{
    public class GeminiJobDescriptionGenerator:IJobDescriptionGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _apiUrl;

        public GeminiJobDescriptionGenerator(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient();


            _apiKey = config["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini API key not configured.");
            _model = config["Gemini:Model"] ?? "gemini-1.5-flash-latest"; // Provide a default
            _apiUrl = config["Gemini:ApiUrl"] ?? "https://generativelanguage.googleapis.com/v1beta/models";
        }

        public async Task<string> GenerateJobDescriptionAsync(string prompt)
        {
            // Construct the full URL dynamically
            var url = $"{_apiUrl}/{_model}:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                new { parts = new[] { new { text = prompt } } }
            }
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Gemini API error: {response.StatusCode}, {responseJson}");
            using var doc = JsonDocument.Parse(responseJson);

            var root = doc.RootElement;

            if (root.TryGetProperty("candidates", out var candidates) &&
                candidates.GetArrayLength() > 0 &&
                candidates[0].TryGetProperty("content", out var contentNode) &&
                contentNode.TryGetProperty("parts", out var parts) &&
                parts.GetArrayLength() > 0 &&
                parts[0].TryGetProperty("text", out var textNode))
            {
                return textNode.GetString() ?? string.Empty;
            }

            return "Job description generation failed: Unexpected response format.";
        }
    }
}
