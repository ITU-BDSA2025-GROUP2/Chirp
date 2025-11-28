using Core;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class UserTimelineModel(ICheepService service) : PageModel
{
    private readonly ICheepService _service = service;
    [BindProperty(SupportsGet = true)] public string Author { get; set; } // Route-bound property

    public required List<CheepViewModel> Cheeps { get; set; }


    public async Task<ActionResult> OnGet([FromQuery] int page = 0)
    {
        // Get UserID and follower ID and their cheeps
        List<Cheep> returnList;
        List<int> followerIds;
        if (Author == User.Identity!.Name)
        {
            followerIds = await _service.GetFollowers(User.Identity.Name);
            var userId = await _service.GetAuthorId(User.Identity.Name);
            followerIds.Add(userId);

            returnList = await _service.GetCheepsFromFollowed(followerIds, page);
        }
        else
        {
            followerIds = await _service.GetFollowers(User.Identity!.Name!);
            returnList = await _service.GetCheepsFromAuthor(Author, page);
        }


        Cheeps = new List<CheepViewModel>();

        foreach (var row in returnList)
        {
            var id = row.Author.AuthorId;
            var isFollowed = false;
           
            foreach(int t in followerIds)
            {   
                if(id == t)
                {
                    isFollowed = true;
                    break;
                }
            }    
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email, isFollowed));
        }

        return Page();
    }


    [BindProperty] public string Text { get; set; }

    public async Task<IActionResult> OnPost()
    {
        var cheep_message = Text;
        string input = User.Identity.Name;
        int index = input.IndexOf("@");
        if (index >= 0)
        {
            input = input.Substring(0, index);
        }

        await _service.CreateCheep(input, User.Identity.Name, cheep_message);

        Cheeps = new List<CheepViewModel>();

        var result = await _service.GetCheeps(0);
        foreach (var row in result)
        {
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email,
                false));
        }

        return RedirectToPage("UserTimeline", new { author = Author });
    }

    [BindProperty] public string Email { get; set; }

    public async Task<IActionResult> OnPostFollow([FromQuery] int page = 0)
    {
        var id = await _service.GetAuthorId(Email);
        var author = await _service.GetEmail(User.Identity.Name, page);
        var IsFollowed = false;

        var followers = await _service.GetFollowers(author.Email);
        foreach (int t in followers)
        {
            if (id == t)
            {
                IsFollowed = true;
                break;
            }
            else
            {
                IsFollowed = false;
            }
        }

        if (!IsFollowed)
        {
            _service.AddFollowerId(author, id);
        }
        else
        {
            _service.RemoveFollowerId(author, id);
        }

        followers = await _service.GetFollowers(User.Identity.Name);

        Console.WriteLine(User.Identity.Name + "You are following these people:");

        foreach (int t in followers)
        {
            Console.WriteLine(t);
        }

        return RedirectToPage("UserTimeline", new { author = Author });
    }
}