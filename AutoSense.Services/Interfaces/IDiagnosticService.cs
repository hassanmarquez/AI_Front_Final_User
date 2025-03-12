using AutoSense.Services.Models;

namespace AutoSense.Services.Interfaces
{
    public interface IDiagnosticService
    {
        Task<List<string>> RequestDiagnosticAsync();

        Task<string> CallAIServiceAsync(ChatRequest message);

        Task<string> NextIteration(List<Message> messages);
    }
}