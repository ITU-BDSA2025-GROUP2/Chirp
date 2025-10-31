using Core;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class PublicModel(ICheepService service) : PageModel
{
    private readonly ICheepService _service = service;
    public required List<CheepViewModel> Cheeps { get; set; }

    public async Task<ActionResult> OnGet([FromQuery] int page = 0)
    {
        Cheeps = new List<CheepViewModel>();
        var result = await _service.GetCheeps(page);
        foreach (var row in result)
        {
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString()));
        }
        
        return Page();
    }
}
