using AutoSense.Services.Interfaces;
using AutoSense.Services.Models;
using AutoSense.Services.Models.cpu_and_mobile;
using AutoSense.Services.Services.Api;
using System.Text;
using System.Text.Json;

namespace AutoSense.Services.Services.Assistant;

public class ApiAssistantService : IAssistantService
{
    HttpClient _client;
    JsonSerializerOptions _serializerOptions;

    public ApiAssistantService()
    {
        _client = new HttpClient();
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<string> GenerateChatResponseAsync(ChatRequest chatMessage)
    {
        Uri uri = new Uri(string.Format(ConstantsApi.BaseUri + ConstantsApi.ChatService, string.Empty));

        try
        {
            string json = JsonSerializer.Serialize(chatMessage, _serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var contentResponse = await response.Content.ReadAsStringAsync();
                var promptResponse = JsonSerializer.Deserialize<ChatResponse>(contentResponse, _serializerOptions);
                return promptResponse!.message!;
            }
            else
            {
                return response.StatusCode.ToString();
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

    }

    public async Task<string> GenerateResponseAsync(string userPrompt)
    {
        Uri uri = new Uri(string.Format(ConstantsApi.BaseUri + ConstantsApi.GeneratePromptService, string.Empty));

        try
        {
            var promptRequest = new PromptRequest()
            {
                llm_type = LlmType.openai.ToString(),
                prompt = userPrompt
            };
            string json = JsonSerializer.Serialize(promptRequest, _serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var contentResponse = await response.Content.ReadAsStringAsync();
                var promptResponse = JsonSerializer.Deserialize<PromptResponse>(contentResponse, _serializerOptions);
                return promptResponse!.message!;
            }
            else
            {
                return response.StatusCode.ToString();
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

}
