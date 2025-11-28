using System.Security.Claims;
using Core;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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
        if (userEmail != null) {
            var authorFromQuery = await GetEmail(userEmail, page);

            if (authorFromQuery == null)
            {
                await CreateAuthor(userEmail, userEmail);   
            }

            
            followers = await GetFollowers(userEmail);
        }
        
        foreach (var row in result)
        {
            var id = row.Author.AuthorId;
            var IsFollowed = false;
           
            foreach(int t in followers)
            {   
                if(id == t)
                {
                    IsFollowed = true;
                    break;
                }
                
            }    
            
            cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email, IsFollowed));

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
}



