namespace FootballSocialMedia.Model.ChatMessages.Dtos.Requests;

// Gönderen (SenderId) artık token'dan alınıyor.
public record SendMessageDto(string ReceiverId, string Message);
