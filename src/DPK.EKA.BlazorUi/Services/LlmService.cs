using DPK.EKA.BlazorUi.Models;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace DPK.EKA.BlazorUi.Services.LlmService
{
    public class LlmService
    {
        private readonly HttpClient _http;

        public HttpClient Client => _http;

        public LlmService(HttpClient http) => _http = http;

        public async Task<ChatResponse> SendMessageAsync(ChatRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/v1/rag/query", request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ChatResponse>() ?? new ChatResponse();
        }

        public async Task<bool> UploadPdfAsync(string fileName, Stream fileStream)
        {
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            content.Add(streamContent, "file", fileName);

            // Keeping this flexible until you map your upload route
            var response = await _http.PostAsync("api/v1/documents/upload", content);
            return response.IsSuccessStatusCode;
        }
    }
}