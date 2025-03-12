using AutoSense.Services.Interfaces;
using AutoSense.Services.Models;
using AutoSense.Services.Services.Assistant;

namespace AutoSense.Services.Services.Diagnostic;

public class DiagnosticService : IDiagnosticService
{
    private readonly MessageHandler _messageHandler;
    private EnumStatus statusDiagnostic;
    private List<string> ErrorCodes;
    private IAssistantService assistantService;

    public EnumStatus Status { get => statusDiagnostic; set => statusDiagnostic = value; }
    public string VIN;

    public DiagnosticService()
    {
        _messageHandler = new MessageHandler();
        statusDiagnostic = EnumStatus.Start;
        ErrorCodes = new List<string>();
        VIN = string.Empty;
        assistantService = GetInstanceAssistantService();
    }

    public DiagnosticService(EnumStatus enumStatus, List<string> codes)
    {
        _messageHandler = new MessageHandler();
        statusDiagnostic = enumStatus;
        ErrorCodes = codes;
        VIN = string.Empty;
        assistantService = GetInstanceAssistantService();
    }
    public IAssistantService GetInstanceAssistantService()
    {
        if (assistantService is null)
            assistantService = new ApiAssistantService(); //ApiAssistantService(); SMLAssistantService();
        return assistantService;
    }

    public async Task<string> NextIteration(List<Message> messages)
    {
        Nextstep();

        if (statusDiagnostic == EnumStatus.Connecting)
        {
            VIN = await RequestVinAsync();
        }
        else if (statusDiagnostic == EnumStatus.ScanVehicle)
        {
            ErrorCodes = await RequestDiagnosticAsync();
        }

        ChatRequest chatMessage = new ChatRequest()
        {
            status = statusDiagnostic.ToString(),
            vin = VIN,
            codes = ErrorCodes,
            messages = messages
        };

        var response = await assistantService.GenerateChatResponseAsync(chatMessage);

        return response;
    }

    private void Nextstep()
    {
        switch (statusDiagnostic)
        {
            case EnumStatus.Start:
                statusDiagnostic = EnumStatus.Connecting;
                break;
            case EnumStatus.Connecting:
                statusDiagnostic = EnumStatus.ScanVehicle;
                break;
            case EnumStatus.ScanVehicle:
                statusDiagnostic = EnumStatus.Explain;
                break;
            case EnumStatus.Explain:
                statusDiagnostic = EnumStatus.Summarize;
                break;
            case EnumStatus.Summarize:
                statusDiagnostic = EnumStatus.Appointment;
                break;
            case EnumStatus.Appointment:
                statusDiagnostic = EnumStatus.Start;
                break;
        }
    }

    public Task<string> CallAIServiceAsync(ChatRequest message)
    {
        
        return assistantService.GenerateChatResponseAsync(message);
    }

    public async Task<List<string>> RequestDiagnosticAsync()
    {
        // Conectar al dispositivo OBD2
        //await _messageHandler.ConnectToOBD2DeviceAsync();

        // Solicitar los códigos de error
        //List<string> errorCodes = await _messageHandler.RequestErrorCodesAsync();
        List<string> errorCodes = new List<string> { "P050D" };
        return await Task.FromResult(errorCodes);
    }

    public async Task<string> RequestVinAsync()
    {
        // Conectar al dispositivo OBD2
        //await _messageHandler.ConnectToOBD2DeviceAsync();

        // Solicitar el VIN
        //string vin = await _messageHandler.RequestVINAsync();
        //return vin;
        return await Task.FromResult("3GNAL7EC3BS671866");
    }

}
