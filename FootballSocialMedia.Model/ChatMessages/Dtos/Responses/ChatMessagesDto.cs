namespace FootballSocialMedia.Model.ChatMessages.Dtos.Responses;

public sealed record ChatMessageDto
{
    public Guid Id { get; init; }
    public string SenderName { get; init; }
    public string ReceiverName { get; init; }
    public string Message { get; init; }
    public DateTime Timestamp { get; init; }
}