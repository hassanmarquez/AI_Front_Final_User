using AutoSense.Services.Models;

namespace AutoSense.Models;

public class DiagnosticMessage
{
    public required RoleEnums Rol { get; set; }
    public required string Text { get; set; }
    public required Color BackgroundColor { get; set; }
    public LayoutOptions HorizontalOptions { get; set; }

    public static List<Message> GetListMessages(List<DiagnosticMessage> diagnosticMessages) 
    {
        List<Message>messages = new List<Message>();
        foreach (var diagnosticMessage in diagnosticMessages)
        {
            messages.Add(new Message() { 
                role = diagnosticMessage.Rol.ToString(), 
                content = diagnosticMessage.Text 
            });
        };
        return messages;
    }
}