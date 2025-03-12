using AutoSense.Services.Interfaces;
using AutoSense.Services.Models;
using Microsoft.ML.OnnxRuntimeGenAI;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace AutoSense.Services.Services.Assistant;

public class SMLAssistantService : IAssistantService
{
    private Model? model = null;
    private Tokenizer? tokenizer = null;
    private string ModelDir = "";
    //private DispatcherQueue dispatcherQueue;

    public SMLAssistantService()
    {
        ModelDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            @"Models/cpu_and_mobile/cpu-int4-awq-block-128-acc-level-4");

        InitializeModel();
    }

    [MemberNotNullWhen(true, nameof(model), nameof(tokenizer))]
    public bool IsReady => model != null && tokenizer != null;

    public void Dispose()
    {
        model?.Dispose();
        tokenizer?.Dispose();
    }

    public void InitializeModel()
    {
        try
        {
            Console.WriteLine("Loading model...");

            var sw = Stopwatch.StartNew();
            model = new Model(ModelDir);
            tokenizer = new Tokenizer(model);
            sw.Stop();

            Console.WriteLine($"Model loading took {sw.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading model: {ex.Message}");
        }
    }


    public IAsyncEnumerable<string> InferStreaming(string systemPrompt, string userPrompt, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var prompt = $@"<|system|>{systemPrompt}<|end|><|user|>{userPrompt}<|end|><|assistant|>";
        return InferStreaming(prompt, ct);
    }

    public IAsyncEnumerable<string> InferStreaming(string systemPrompt, List<Message> history, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var prompt = $@"<|system|>{systemPrompt}<|end|>";
        foreach (var message in history)
        {
            prompt += $"<|{message.role.ToString().ToLower()}|>{message.content}<|end|>";
        }
        prompt += "<|assistant|>";

        return InferStreaming(prompt, ct);

    }

    public async IAsyncEnumerable<string> InferStreaming(string prompt, [EnumeratorCancellation] CancellationToken ct = default)
    {
        if (!IsReady)
        {
            throw new InvalidOperationException("Model is not ready");
        }

        var generatorParams = new GeneratorParams(model);

        var sequences = tokenizer.Encode(prompt);

        generatorParams.SetSearchOption("max_length", 1024);
        generatorParams.SetInputSequences(sequences);
        generatorParams.TryGraphCaptureWithMaxBatchSize(1);

        using var tokenizerStream = tokenizer.CreateStream();
        using var generator = new Generator(model, generatorParams);
        StringBuilder stringBuilder = new();
        while (!generator.IsDone())
        {
            string part;
            try
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                generator.ComputeLogits();
                generator.GenerateNextToken();
                part = tokenizerStream.Decode(generator.GetSequence(0)[^1]);
                stringBuilder.Append(part);
                if (stringBuilder.ToString().Contains("<|end|>")
                    || stringBuilder.ToString().Contains("<|user|>")
                    || stringBuilder.ToString().Contains("<|system|>"))
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                break;
            }

            yield return part;
        }
    }

    public async Task<string> GenerateChatResponseAsync(ChatRequest chatMessage)
    {
        string systemPrompt = GetSystemPromptAsync(chatMessage);
        string result = string.Empty;

        await foreach (var messagePart in InferStreaming(systemPrompt, chatMessage.messages))
        {
            var part = messagePart;
            //dispatcherQueue.TryEnqueue(() =>
            //{
                part = messagePart.TrimStart();
                result += part;
            //});
        }

        return result;
    }

    private string GetSystemPromptAsync(ChatRequest assistantmessages)
    {
        var systemPrompt = "You are an AI assistant specialized in vehicle diagnostics, particularly in interpreting OBD-II message codes.";
        switch (assistantmessages.status)
        {
            case "Connecting":
                systemPrompt = $@"You are an AI assistant specializing in vehicle diagnostics. 
                                    Your first task is to kindly greet the user and inform him that you will start the connection with the vehicle using the OBD2 port through a Bluetooth connection.
                                    Then you must inform them that you found a vehicle with the VIN '{assistantmessages.vin}', explain what type of vehicle this VIN code corresponds to
                                    Guidelines:
                                    Style: Maintain a formal tone in all interactions.
                                    User Knowledge Level: Adapt explanations based on simple terms and provide basic explanations.
                                    Expertise: Demonstrate in-depth knowledge of vehicle systems and OBD-II codes.";
                break;

            case "ScanVehicle":
            case "Explain":
                systemPrompt = $@"You are an AI assistant specialized in vehicle diagnostics, particularly in interpreting OBD-II message codes. 
                                    Your role is to assist users by performing a triage of the provided OBD-II code, offering information about the code’s implications, and indicating the severity of the issue using a traffic light system. 
                                    This system will help users understand whether they need to visit a service center immediately if they should stop driving the vehicle, or if the issue is not urgent.
                                    Triage: Use the OBD-II code to assess the situation and provide a clear recommendation:
                                    Red: Serious issue, stop the vehicle immediately and seek assistance.
                                    Yellow: Caution, visit a service center soon.
                                    Green: No immediate action is required, the issue is not critical.
                                    The vehicle has a list of error codes:
                                    {string.Join(", ", assistantmessages.codes)}
                                    Guidelines:
                                    Style: Maintain a formal tone in all interactions.
                                    User Knowledge Level: Adapt explanations based on simple terms and provide basic explanations.
                                    Expertise: Demonstrate in-depth knowledge of vehicle systems and OBD-II codes.";
                break;

            case "Summarize":
                systemPrompt = @"You are an AI assistant specialized in vehicle diagnostics, particularly in interpreting OBD-II message codes. 
                                    you should summarize the issue found, offering information about the code’s implications, and indicating the severity of the issue using a traffic light system. 
                                    This system will help users understand whether they need to visit a service center immediately, 
                                    if they should stop driving the vehicle, or if the issue is not urgent.
                                    Red: Serious issue, stop the vehicle immediately and seek assistance.
                                    Yellow: Caution, visit a service center soon.
                                    Green: No immediate action required, the issue is not critical.
                                    Guidelines:
                                    Style: Maintain a formal tone in all interactions.
                                    User Knowledge Level: Adapt explanations based on simple terms and provide basic explanations.
                                    Expertise: Demonstrate in-depth knowledge of vehicle systems and OBD-II codes.";
                break;

            case "Appointment":
                double latitude = 4.678967;
                double longitude = -74.044615;
                systemPrompt = $@"You are an AI assistant specializing in vehicle diagnostics.
                                    you should offer to schedule an appointment at the nearest service center to address the issue found.
                                    The user's current location is latitude {latitude} and longitude {longitude}.
                                    Guidelines:
                                    Style: Maintain a formal tone in all interactions.
                                    User Knowledge Level: Adapt explanations based on simple terms and provide basic explanations.";
                break;

            default: // "Start"
                systemPrompt = @"You are an AI assistant specializing in vehicle diagnostics. 
                                    Your first task is to kindly greet the user and inform him that you will start the connection with the vehicle using the OBD2 port through a Bluetooth connection.
                                    Guidelines:
                                    Style: Maintain a formal tone in all interactions.
                                    User Knowledge Level: Adapt explanations based on simple terms and provide basic explanations.
                                    Expertise: Demonstrate in-depth knowledge of vehicle systems and OBD-II codes.";
                break;
        }
        return systemPrompt;
    }

    public async Task<string> GenerateResponseAsync(string userPrompt)
    {
        string result = string.Empty;

        if (model != null)
        {
            var systemPrompt = "You are a helpful assistant.";

            var prompt = $@"<|system|>{systemPrompt}<|end|><|user|>{userPrompt}<|end|><|assistant|>";

            await foreach (var part in InferStreaming(prompt))
            {
                Console.Write(part);
                result += part;
            }
        }

        return result;
    }

    public async IAsyncEnumerable<string> InferStreaming(string prompt)
    {
        if (model == null || tokenizer == null)
        {
            throw new InvalidOperationException("Model is not ready");
        }

        var generatorParams = new GeneratorParams(model);

        var sequences = tokenizer.Encode(prompt);

        generatorParams.SetSearchOption("max_length", 2048);
        generatorParams.SetInputSequences(sequences);
        generatorParams.TryGraphCaptureWithMaxBatchSize(1);

        using var tokenizerStream = tokenizer.CreateStream();
        using var generator = new Generator(model, generatorParams);
        StringBuilder stringBuilder = new();
        while (!generator.IsDone())
        {
            string part;
            try
            {
                //await Task.Delay(10).ConfigureAwait(false);
                generator.ComputeLogits();
                generator.GenerateNextToken();
                part = tokenizerStream.Decode(generator.GetSequence(0)[^1]);
                stringBuilder.Append(part);
                if (stringBuilder.ToString().Contains("<|end|>")
                    || stringBuilder.ToString().Contains("<|user|>")
                    || stringBuilder.ToString().Contains("<|system|>"))
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                break;
            }

            yield return part;
        }
    }


}