namespace FootballSocialMedia.Model.ChatMessages.Dtos.Requests;

public record SendMessageDto(string SenderId, string ReceiverId, string Message);
