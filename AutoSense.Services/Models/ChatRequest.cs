namespace AutoSense.Services.Models;

public class ChatRequest
{
    public string status { get; set; }
    public string vin { get; set; }
    public List<string> codes { get; set; }
    public List<Message> messages { get; set; }
}

public class Message
{
    public string role { get; set; }
    public string content { get; set; }
}