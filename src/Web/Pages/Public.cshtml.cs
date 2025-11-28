using System.Globalization;
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
        Cheeps = await _service.GetAllCheeps(User.Identity.Name, page);
        return Page();
    }



    [BindProperty]
    public string Text { get; set; }

    public async Task<IActionResult> OnPost()
    {
        

        var cheep_message = Text;
       
        if (cheep_message.Length < 161)
        {
            //Username fix when scaffolding is doen. 
            await _service.CreateCheep(User.Identity.Name, User.Identity.Name, cheep_message);
        }

        return RedirectToPage("");
    }

    [BindProperty]
    public string Email { get; set; }

    //TODO make redirect so you stay on the current page even if its >0
    public async Task<IActionResult> OnPostFollow([FromQuery] int page = 0)
    {
        await _service.UpdateFollower(User.Identity.Name, Email);

        return RedirectToPage("");
    } 
    [BindProperty]
    public int CheepID { get; set; }
    public async Task<IActionResult> OnPostLike([FromQuery] int page = 0)
    {

        _service.UpdateCheepLikes(CheepID, User.Identity.Name);

        return RedirectToPage("");
    }


    
}
