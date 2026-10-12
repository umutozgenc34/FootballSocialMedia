namespace FootballSocialMedia.Model.ChatMessages.Dtos.Responses;

public sealed record ConversationDto
{
    public string UserId { get; init; } = default!;
    public string UserName { get; init; } = default!;
    public string? ProfileImageUrl { get; init; }
    public string LastMessage { get; init; } = default!;
    public DateTime LastMessageTime { get; init; }
    public bool LastMessageIsMine { get; init; }
}
