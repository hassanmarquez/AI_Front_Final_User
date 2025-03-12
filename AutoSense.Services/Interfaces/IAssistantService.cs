using AutoSense.Services.Models;

namespace AutoSense.Services.Interfaces;

public interface IAssistantService
{
    Task<string> GenerateResponseAsync(string userPrompt);

    Task<string> GenerateChatResponseAsync(ChatRequest chatMessage);
}
