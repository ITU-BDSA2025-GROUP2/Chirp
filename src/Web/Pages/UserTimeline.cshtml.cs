using System.Globalization;
using Core;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class UserTimelineModel(ICheepService service) : PageModel
{
    public required List<CheepViewModel> Cheeps { get; set; }

    public async Task<ActionResult> OnGet(string author, [FromQuery] int page)
    {
        var returnList = await service.GetCheepsFromAuthor(author, page);
        Cheeps = new List<CheepViewModel>();
        foreach (var row in returnList)
        {
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(CultureInfo.CurrentCulture)));
        }


        return Page();
    }


    [BindProperty]
    public required string Text { get; set; }

    public async Task<IActionResult> OnPost()
    {
        var cheepMessage = Text;
        string input = User.Identity!.Name!;
        int index = input.IndexOf("@", StringComparison.Ordinal);
        if (index >= 0)
        {
            input = input.Substring(0, index);
        }
        await service.CreateCheep(input, User.Identity.Name!, cheepMessage);


        Cheeps = new List<CheepViewModel>();

        var result = await service.GetCheeps(0);
        foreach (var row in result)
        {
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(CultureInfo.CurrentCulture)));
        }

        return RedirectToPage("Public");
    }
}
