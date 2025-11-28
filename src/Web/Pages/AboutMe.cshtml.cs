using Core;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class AboutMeModel(ICheepService service) : PageModel
{
    private readonly ICheepService _service = service;
    public required List<CheepViewModel> Cheeps { get; set; }
    public required AuthorViewModel Author { get; set; }
    public required List<AuthorViewModel> Following { get; set; }
    public required List<CheepViewModel> LikedCheeps { get; set; }

    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        Cheeps = await _service.GetUserCheeps(User.Identity!.Name!, page);
        Author =  await _service.GetAuthorViewModel(User.Identity!.Name!);
        Following = await _service.GetFollowerViewModel(User.Identity!.Name!);
        LikedCheeps = await _service.GetLikedCheepsForAuthor(User.Identity!.Name!);
        
        return Page();
    }

    public async Task<IActionResult> OnPostForget()
    {
        var identity = User.Identity!.Name;
        await _service.DeleteAuthor(identity!);
        
        Response.Cookies.Delete(".AspNetCore.Identity.Application");
        Response.Cookies.Delete("Seq-Session");
        Response.Cookies.Delete(".AspNetCore.Antiforgery.xYiNViD5USA");
        
        return RedirectToPage("Public");
    }
    
    [BindProperty] public required string Email { get; set; }

    public async Task<IActionResult> OnPostFollow([FromQuery] int page = 0)
    {
        await _service.UpdateFollower(User.Identity!.Name!, Email);
        return RedirectToPage("AboutMe");
    }

    [BindProperty]
    public int CheepID { get; set; }
    public async Task<IActionResult> OnPostLike([FromQuery] int page = 0)
    {

        _service.UpdateCheepLikes(CheepID, User.Identity.Name);

        return RedirectToPage("AboutMe");
    }
}
