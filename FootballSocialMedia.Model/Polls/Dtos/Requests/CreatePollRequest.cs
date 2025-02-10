namespace FootballSocialMedia.Model.Polls.Dtos.Requests;

public sealed record CreatePollRequest(string Question, List<string> Options, DateTime ExpiryDate);
