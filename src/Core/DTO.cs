public record CheepViewModel(int CheepId, string Author, string Message, string Timestamp, string Email, bool IsFollowed, List<int> Likes);
public record AuthorViewModel(string Author, string Email);