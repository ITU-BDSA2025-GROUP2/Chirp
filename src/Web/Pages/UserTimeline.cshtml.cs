using Chirp.Razor.Chirp.Infrastructure.Chirp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel(ICheepService service) : PageModel
{
    private readonly ICheepService _service = service;
    public required List<CheepViewModel> Cheeps { get; set; }

    public async Task<ActionResult> OnGet(string author, [FromQuery] int page)
    {
        var returnList = await _service.GetCheepsFromAuthor(author, page);
        Cheeps = new List<CheepViewModel>();
        foreach (var row in returnList)
        {
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString()));
        }

        
        return Page();
    }
}
