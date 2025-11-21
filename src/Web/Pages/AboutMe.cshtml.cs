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
    public required Author Author { get; set; }

    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        var user = User.Identity!.Name;
        Author = await _service.GetEmail(user!, 0);
        
        int index = user.IndexOf("@")!;
        string username = "";
        if (index >= 0)
        {
            username = user.Substring(0, index);
        }
        var returnList = await _service.GetCheepsFromAuthor(username, page);
        Cheeps = new List<CheepViewModel>();
        foreach (var row in returnList)
        {
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email, ""));
        }
        return Page();
    }

    public async Task<IActionResult> OnPostForget()
    {
        var identity = User.Identity.Name;
        
        await _service.DeleteAuthor(identity);
        
        Response.Cookies.Delete(".AspNetCore.Identity.Application");
        Response.Cookies.Delete("Seq-Session");
        Response.Cookies.Delete(".AspNetCore.Antiforgery.xYiNViD5USA");
        
        return RedirectToPage("Public");
    }
}
