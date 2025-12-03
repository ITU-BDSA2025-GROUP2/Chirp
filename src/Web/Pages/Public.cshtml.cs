using System.Security.Claims;
using Core;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class PublicModel(ICheepService service) : PageModel
{
    private readonly ICheepService _service = service;
    public required List<CheepViewModel> Cheeps { get; set; }


    public async Task<ActionResult> OnGet([FromQuery] int page = 0)
    {
        Cheeps = await _service.GetAllCheeps(User.Identity!.Name!, User.FindFirst(ClaimTypes.Email)?.Value!, page);
        return Page();
    }



    [BindProperty]
    public required string Text { get; set; }

    public async Task<IActionResult> OnPost()
    {
        var cheepMessage = Text;
       
        if (cheepMessage.Length < 161)
        {
            await _service.CreateCheep(User.Identity!.Name!, User.FindFirst(ClaimTypes.Email)?.Value!, cheepMessage);
        }

        return RedirectToPage("");
    }

    [BindProperty]
    public required string Email { get; set; }

    public async Task<IActionResult> OnPostFollow([FromQuery] int page = 0)
    {
        await _service.UpdateFollower(User.FindFirst(ClaimTypes.Email)?.Value!, Email);

        return RedirectToPage("");
    } 
    [BindProperty]
    public int CheepId { get; set; }
    public async Task<IActionResult> OnPostLike([FromQuery] int page = 0)
    {
        await _service.UpdateCheepLikes(CheepId, User.FindFirst(ClaimTypes.Email)?.Value!);

        return RedirectToPage("");
    }


    
}
