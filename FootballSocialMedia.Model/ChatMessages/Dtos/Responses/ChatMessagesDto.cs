namespace FootballSocialMedia.Model.ChatMessages.Dtos.Responses;

public sealed record ChatMessageDto
{
    public Guid Id { get; init; }
    public string SenderId { get; init; } = default!;
    public string ReceiverId { get; init; } = default!;
    public string? SenderName { get; init; }
    public string? ReceiverName { get; init; }
    public string Message { get; init; } = default!;
    public DateTime Timestamp { get; init; }
}
