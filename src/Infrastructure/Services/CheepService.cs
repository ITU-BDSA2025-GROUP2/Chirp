using Core;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class CheepService : ICheepService
{
    private CheepRepository _cheepRepository;
    private AuthorRepository _authorRepository;
    public CheepService(ChatDbContext dbContext)
    {
       
        //_dbcontext = dbContext;
        _authorRepository = new AuthorRepository(dbContext);
        _cheepRepository = new CheepRepository(dbContext);
    }

    public async Task<List<Cheep>> GetCheeps(int page)
    {
        return await _cheepRepository.ReadCheeps(page);
    }

    public async Task<List<Cheep>> GetCheepsFromAuthor(string author, int page)
    {
        // filter by the provided author name
        return await _cheepRepository.ReadCheepsPerson(author, page);
    }

    public async Task<List<Cheep>> GetCheepsFromFollowed(List<int> follows, int page = 0)
    {
        // filter by the provided author name
        return await _cheepRepository.ReadCheepsFollowed(follows, page);
    }


    public async Task<Author> GetAuthor(string author, int page)
    {
        return await _authorRepository.ReturnBasedOnNameAsync(author, page);
    }


     public async Task<int> GetAuthorId(string email)
     {
         if (email == null);
        return await _authorRepository.ReturnAuthorsId(email);
    }

    public async Task<Author> GetEmail(string email, int page)
    {
        var result = await _authorRepository.ReturnBasedOnEmailAsync(email, page);
        try
        {
            return result[0];
        }
        catch
        {
            return null;
        }
    }


    public async Task<List<int>> GetFollowers(string email)
    {
        return await _authorRepository.ReturnFollowAuthorsIds(email);
    }

    public async Task CreateCheep(string author, string email, string msg)
    {
        await _cheepRepository.CreateCheep(author, email, msg);
    }

    public async Task CreateAuthor(string author, string email)
    {
        _authorRepository.CreateAuthor(author, email);
    }


    public void AddFollowerId(Author author, int id)
    {
        _authorRepository.AddFollowerId(author, id);
    }

    public void RemoveFollowerId(Author author, int id)
    {
        _authorRepository.RemoveFollowerId(author, id);
    }


    public async Task DeleteAuthor(string email)
    {
        
        await _authorRepository.DeleteAuthor(email);
    }

    public async Task<List<CheepViewModel>> GetAllCheeps(string userEmail, int page) //user, page
    {
        var cheeps = new List<CheepViewModel>();
        var result = await _cheepRepository.ReadCheeps(page);
        
      
        var followers = new List<int>();
        var userId = -1;
        if (userEmail != null) {
            var authorFromQuery = await GetEmail(userEmail, page);

            if (authorFromQuery == null)
            {
                await CreateAuthor(userEmail, userEmail);   
            }

            userId = await GetAuthorId(userEmail);
            followers = await GetFollowers(userEmail);
        }
        
        foreach (var cheep in result)
        {
            var id = cheep.Author.AuthorId;
            var isFollowed = false;
           
            foreach(int t in followers)
            {   
                if(id == t)
                {
                    isFollowed = true;
                    break;
                }
                
            }    
            
            
            var isLiked = cheep.PeopleLikes.Contains(userId);
            cheeps.Add(new CheepViewModel(cheep.CheepId, cheep.Author.Name, cheep.Text, cheep.TimeStamp.ToString(), cheep.Author.Email, 
                isFollowed, await GetCheepLikesAmount(cheep.CheepId), isLiked));
        }

        return cheeps;
    }

    public async Task UpdateFollower(string userEmail, string followerEmail)
    {
        var id = await _authorRepository.ReturnAuthorsId(followerEmail);
        var author = await GetEmail(userEmail, 0);
        var IsFollowed = false;

        var followers = await GetFollowers(userEmail);
        foreach(int t in followers)
        {   
            if(id == t)
            {

                _authorRepository.RemoveFollowerId(author, id);
                return;
            }
            
            
        }
        _authorRepository.AddFollowerId(author, id);
    }

    public async Task<List<CheepViewModel>> GetUserTimelineCheeps(string userEmail, string userTimelineAuthor, int page)
    {
        // Get UserID and follower ID and their cheeps
        List<Cheep> cheepsList;
        List<int> followerIds;
        var userId = -1;
        if (userTimelineAuthor == userEmail)
        {
            followerIds = await GetFollowers(userEmail);
            userId = await GetAuthorId(userEmail);
            followerIds.Add(userId);
            cheepsList = await GetCheepsFromFollowed(followerIds, page);
        }
        else
        {
            followerIds = await GetFollowers(userEmail);
            cheepsList = await GetCheepsFromAuthor(userTimelineAuthor, page);
        }

        // Add all cheeps into a CheepViewModel and return it
        var cheeps = new List<CheepViewModel>();
        foreach (var cheep in cheepsList)
        {
            var id = cheep.Author.AuthorId;
            var isFollowed = false;
           
            foreach(int t in followerIds)
            {   
                if(id == t)
                {
                    isFollowed = true;
                    break;
                }
            }    
            
            
            var isLiked = cheep.PeopleLikes.Contains(userId);
            cheeps.Add(new CheepViewModel(cheep.CheepId, cheep.Author.Name, cheep.Text, cheep.TimeStamp.ToString(), cheep.Author.Email, 
                isFollowed, await GetCheepLikesAmount(cheep.CheepId), isLiked));
        }

        return cheeps;
    }


    public async Task<List<CheepViewModel>> GetUserCheeps(string userEmail, int page)
    {
        var userCheeps = await GetCheepsFromAuthor(userEmail, page);
        var cheeps = new List<CheepViewModel>();
        foreach (var cheep in userCheeps)
        {
            var userId = await GetAuthorId(userEmail);
            var isLiked = cheep.PeopleLikes.Contains(userId);
            
            cheeps.Add(new CheepViewModel(cheep.CheepId, cheep.Author.Name, cheep.Text, cheep.TimeStamp.ToString(), cheep.Author.Email, 
                false, await GetCheepLikesAmount(cheep.CheepId), isLiked));
        }

        return cheeps;
    }

    public async Task<AuthorViewModel> GetAuthorViewModel(string email)
    {
        var author = await GetAuthor(email, 0);
        var authorViewModel = new AuthorViewModel(author.Name, author.Email);

        return authorViewModel;
    }

    public async Task<List<AuthorViewModel>> GetFollowerViewModel(string email)
    {
        var followerIds = await GetFollowers(email);
        var followerViewModels = new List<AuthorViewModel>();
        
        var followers = await _authorRepository.GetAuthorsFromIdList(followerIds);
        foreach (var followerAuthor in followers)
        {
            followerViewModels.Add(new AuthorViewModel(followerAuthor.Name, followerAuthor.Email));
        }
        
        return followerViewModels;
    }
    
    public async Task UpdateCheepLikes(int cheepId, string userEmail)
    {
        var cheep = await _cheepRepository.GetCheepFromId(cheepId);
        var author = await GetEmail(userEmail, 0);

        var cheepLikes = await _cheepRepository.GetLikedAuthors(cheepId);
        foreach(int t in cheepLikes)
        {   
            if(author.AuthorId == t)
            {
                _cheepRepository.RemovelikedId(cheep, author.AuthorId);
                return;
            }
        }
        _cheepRepository.AddlikedId(cheep, author.AuthorId);
    }

    public async Task<List<int>> GetCheepLikesAmount(int cheepId)
    {
        return await _cheepRepository.GetLikedAuthors(cheepId);
    }

    public async Task<List<int>> GetAuthorsLikes(string email)
    {
        return null;
    }
    
}



