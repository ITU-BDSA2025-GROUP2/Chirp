using System.Security.Claims;
using Core;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class UserTimelineModel(ICheepService service) : PageModel
{
    [BindProperty(SupportsGet = true)] public required string Author { get; set; } // Route-bound property

    public required List<CheepViewModel> Cheeps { get; set; }


    public async Task<ActionResult> OnGet([FromQuery] int page = 0)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var author = await service.GetAuthorFromName(Author, 0);
        
        Cheeps = await service.GetUserTimelineCheeps(userEmail!, author, page);
        return Page();
    }


    [BindProperty] public required string Text { get; set; }

    public async Task<IActionResult> OnPost()
    {
        await service.CreateCheep(User.Identity!.Name!, User.FindFirst(ClaimTypes.Email)?.Value!, Text);

        return RedirectToPage("UserTimeline", new { author = Author });
    }

    [BindProperty] public required string Email { get; set; }

    public async Task<IActionResult> OnPostFollow([FromQuery] int page = 0)
    {
        await service.UpdateFollower(User.FindFirst(ClaimTypes.Email)?.Value!, Email);
        return RedirectToPage("UserTimeline", new { author = Author });
    }

    [BindProperty]
    public int CheepId { get; set; }
    public async Task<IActionResult> OnPostLike([FromQuery] int page = 0)
    {
        await service.UpdateCheepLikes(CheepId, User.FindFirst(ClaimTypes.Email)?.Value!);

        return RedirectToPage("UserTimeline", new { author = Author });
    }
}