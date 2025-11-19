using Core;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class AboutMeModel(ICheepService service) : PageModel
{
    private readonly ICheepService _service = service;
    public required List<CheepViewModel> Cheeps { get; set; }

    public async Task<ActionResult> OnGet(string author, [FromQuery] int page)
    {

        int index = author.IndexOf("@");
        if (index >= 0)
        {
            author = author.Substring(0, index);
        }
        var returnList = await _service.GetCheepsFromAuthor(author, page);
        Cheeps = new List<CheepViewModel>();
        foreach (var row in returnList)
        {
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString()));
        }


        return Page();
    }


    
}
