using System.Threading.Tasks;
using Core;
using Infrastructure.Services;
using Microsoft.AspNetCore.Components.Forms;
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
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email));
        }

        return Page();
    }



    [BindProperty]
    public string Text { get; set; }

    public async Task<IActionResult> OnPost()
    {
        var cheep_message = Text;
        string input = User.Identity.Name;
        int index = input.IndexOf("@");
        if (index >= 0)
        {
            input = input.Substring(0, index);
        }
        await _service.CreateCheep(input, User.Identity.Name, cheep_message);


        Cheeps = new List<CheepViewModel>();

        var result = await _service.GetCheeps(0);
        foreach (var row in result)
        {
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email));
        }



        return Page();
    }

    [BindProperty]
    public string Email { get; set; }

    public async Task<IActionResult> OnPostFollow([FromQuery] int page = 0)
    {

        
        var id = await _service.GetAuthorId(Email);

        Console.WriteLine(User.Identity.Name + " Is now following "+ Email + " test: " + id);


        


        // Populate Cheeps (copy from OnGet/OnPost)
        Cheeps = new List<CheepViewModel>();
        var result = await _service.GetCheeps(page);
        foreach (var row in result)
        {
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email));
        }


        return Page();
    } 


}
