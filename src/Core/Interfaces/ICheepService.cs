using Core.Model;

namespace Core.Interfaces;

public interface ICheepService
{
    public Task<List<Cheep>> GetCheeps(int page);
    public Task<List<Cheep>> GetCheepsFromAuthorId(int authorId, int page);
    public Task<List<Cheep>> GetCheepsFromFollowed(List<int> follows, int page);
    public Task<Author> GetAuthorFromName(string authorName, int page);
    public Task<int> GetAuthorId(string email);
    public Task<Author?> GetEmail(string email, int page);
    public Task<List<int>> GetFollowers(string email);
    public Task CreateCheep(string author, string email, string msg);
    public void CreateAuthor(string author, string email);
    public void AddFollowerId(Author author, int id);
    public void RemoveFollowerId(Author author, int id);
    public Task DeleteAuthor(string email);

    public Task<List<CheepViewModel>> GetAllCheeps(string name, string userEmail, int page);

    public Task UpdateFollower(string userEmail, string followerEmail);

    public Task<List<CheepViewModel>> GetUserTimelineCheeps(string userEmail, Author userTimelineAuthor, int page);

    public Task<List<CheepViewModel>> GetUserCheeps(string userEmail, int page);

    public Task<AuthorViewModel> GetAuthorViewModel(string email);

    public Task<List<AuthorViewModel>> GetFollowerViewModel(string email);

    public Task UpdateCheepLikes(int cheepId, string userEmail);
    public Task<List<int>> GetCheepLikesAmount(int cheepId);
    public Task<List<CheepViewModel>> GetLikedCheepsForAuthor(string userEmail);

    public Task<Author> GetAuthorFromEmail(string authorEmail, int page);
}