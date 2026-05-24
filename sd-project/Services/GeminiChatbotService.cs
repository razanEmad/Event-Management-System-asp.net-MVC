using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventBooking.Services
{
    public class GeminiChatbotService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiChatbotService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Get the API key from appsettings.json
            _apiKey = configuration["GeminiApiKey"] ?? string.Empty;
        }

        public async Task<string> GetResponseAsync(string userMessage)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return "عذراً، لم يتم إعداد مفتاح API (GeminiApiKey) في ملف appsettings.json بعد. يرجى إضافته ليعمل الذكاء الاصطناعي.";
            }

            var prompt = $@"
You are a helpful customer support assistant for an Event Booking website called 'EventBooking'.
Answer the user's questions nicely and briefly. If they ask about things outside of events or booking, politely decline.
User: {userMessage}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            
            // Call Gemini API
            var response = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseString);
                
                try
                {
                    var textResponse = jsonDoc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text").GetString();

                    return textResponse ?? "Sorry, I couldn't generate a response.";
                }
                catch
                {
                    return "Error parsing the response from the AI.";
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return $"Error ({response.StatusCode}): {errorContent}";
        }
    }
}
