using Core;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class UserTimelineModel(ICheepService service) : PageModel
{
    private readonly ICheepService _service = service;
    [BindProperty(SupportsGet = true)] public required string Author { get; set; } // Route-bound property

    public required List<CheepViewModel> Cheeps { get; set; }


    public async Task<ActionResult> OnGet([FromQuery] int page = 0)
    {
        Cheeps = await _service.GetUserTimelineCheeps(User.Identity!.Name!, Author, page);
        return Page();
    }


    [BindProperty] public required string Text { get; set; }

    public async Task<IActionResult> OnPost()
    {
        await _service.CreateCheep(User.Identity!.Name!, User.Identity!.Name!, Text);

        return RedirectToPage("UserTimeline", new { author = Author });
    }

    [BindProperty] public required string Email { get; set; }

    public async Task<IActionResult> OnPostFollow([FromQuery] int page = 0)
    {
        await _service.UpdateFollower(User.Identity!.Name!, Email);
        return RedirectToPage("UserTimeline", new { author = Author });
    }

    [BindProperty]
    public int CheepID { get; set; }
    public async Task<IActionResult> OnPostLike([FromQuery] int page = 0)
    {

        _service.UpdateCheepLikes(CheepID, User.Identity.Name);

        return RedirectToPage("UserTimeline", new { author = Author });
    }
}