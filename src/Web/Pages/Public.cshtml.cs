using Chirp.Razor.Chirp.Infrastructure.Chirp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public async Task<ActionResult> OnGet([FromQuery] int page = 1)
    {
        var result = await _service.GetCheeps(page);
        Cheeps = new List<CheepViewModel>();
        foreach (var row in result)
        {
           
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString()));
        }
        
        return Page();
    }
}
