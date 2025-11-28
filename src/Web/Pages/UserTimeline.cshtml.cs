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
        var author = await _service.GetAuthor(User.Identity.Name, page);
        // Author is automatically populated from the route
        var ids = await _service.GetFollowers(author.Email);
        var returnList = await _service.GetCheepsFromFollowed(ids, page);

        Cheeps = new List<CheepViewModel>();
        var IsFollowed = false;

        if (Author == User.Identity.Name)
        {
            foreach (var row in returnList)
            {
                var id = row.Author.AuthorId;
                IsFollowed = false;

                foreach (int t in ids)
                {
                    if (id == t)
                    {
                        IsFollowed = true;
                        break;
                    }
                }

                if (IsFollowed)
                {
                    Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email,
                        "Unfollow"));
                    continue;
                }

                Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email,
                    "Follow"));
            }
        }

        returnList = await _service.GetCheepsFromAuthor(Author, page);
        foreach (var row in returnList)
        {
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email,
                "Follow"));
        }

        return Page();
    }


    [BindProperty] public string Text { get; set; }

    public async Task<IActionResult> OnPost()
    {
        var cheep_message = Text;
        
        var author = await _service.GetAuthor(User.Identity.Name, 0);

        await _service.CreateCheep(User.Identity.Name, author.Email, cheep_message);

        Cheeps = new List<CheepViewModel>();

        var result = await _service.GetCheeps(0);
        foreach (var row in result)
        {
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email,
                "Follow"));
        }

        return RedirectToPage("UserTimeline", new { author = Author });
    }

    [BindProperty] public string Email { get; set; }

    public async Task<IActionResult> OnPostFollow([FromQuery] int page = 0)
    {
        var id = await _service.GetAuthorId(Email);
        var author = await _service.GetAuthor(User.Identity.Name, page);
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

        followers = await _service.GetFollowers(author.Email);

        Console.WriteLine(User.Identity.Name + "You are following these people:");

        foreach (int t in followers)
        {
            Console.WriteLine(t);
        }
        
        return RedirectToPage("UserTimeline", new { author = Author});
    }
}