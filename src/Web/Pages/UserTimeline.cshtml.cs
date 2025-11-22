using Core;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class UserTimelineModel(ICheepService service) : PageModel
{
    private readonly ICheepService _service = service;
    [BindProperty(SupportsGet = true)]
    public string Author { get; set; } // Route-bound property
    
    public required List<CheepViewModel> Cheeps { get; set; }
    

    public async Task<ActionResult> OnGet([FromQuery] int page = 0)
    {
        // Author is automatically populated from the route
        var ids = await _service.GetFollowers(User.Identity.Name);
        var returnList = await _service.GetCheepsFromFollowed(ids, page);

        Cheeps = new List<CheepViewModel>();
        var IsFollowed = false;

        if (Author == User.Identity.Name)
        {
            foreach (var row in returnList)
            {
                IsFollowed = ids.Contains(row.Author.AuthorId);
                Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email, IsFollowed ? "Unfollow" : "Follow"));
            }
        }

        returnList = await _service.GetCheepsFromAuthor(Author, page);
        foreach (var row in returnList)
        {
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email, "Follow"));
        }

        return Page();
    }


    [BindProperty]
    public string Text { get; set; }

    public async Task<IActionResult> OnPost()
    {
        var input = User.Identity.Name.Split('@')[0];
        await _service.CreateCheep(input, User.Identity.Name, Text);
        return RedirectToPage("UserTimeline", new { author = Author });
    }

    [BindProperty]
    public string Email { get; set; }

    public async Task<IActionResult> OnPostFollow([FromQuery] int page = 0)
    {
        var id = await _service.GetAuthorId(Email);
        var author = await _service.GetEmail(User.Identity.Name, page);

        var followers = await _service.GetFollowers(author.Email);
        if (followers.Contains(id))
            _service.RemoveFollowerId(author, id);
        else
            _service.AddFollowerId(author, id);

        return RedirectToPage("UserTimeline", new { author = Author });
    }
}
