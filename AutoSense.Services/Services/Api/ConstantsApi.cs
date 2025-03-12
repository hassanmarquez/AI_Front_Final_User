namespace AutoSense.Services.Services.Api;

public static class ConstantsApi
{
    public const string BaseUri = "https://api-assistant.azurewebsites.net/";

    public const string GeneratePromptService = "assistant/prompt";

    public const string ChatService = "assistant/prompt_messages";
}

public enum LlmType
{
    openai,
    gemini,
    anthropic,
    llama
}