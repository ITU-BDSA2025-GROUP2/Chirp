using Core;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class UserTimelineModel(ICheepService service) : PageModel
{
    private readonly ICheepService _service = service;
    public required List<CheepViewModel> Cheeps { get; set; }

    public async Task<ActionResult> OnGet(string author, [FromQuery] int page)
    {

        //This would be nice to order all by timestamp instead of person. 
        var ids = await _service.GetFollowers(User.Identity.Name);
        var returnList = await _service.GetCheepsFromFollowed(ids, page);
        var IsFollowed = false;
        Cheeps = new List<CheepViewModel>();
        if (author == User.Identity.Name)
        {   
            foreach (var row in returnList)
            {
                var id = row.Author.AuthorId;
                IsFollowed = false;

                foreach(int t in ids)
                {   
                    if(id == t)
                    {
                        IsFollowed = true;
                        break;
                    }
                    
                }
                if (IsFollowed)
                {
                    Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email, "Unfollow"));
                    continue;
                }
                Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email, "Follow"));

            }
        }
        returnList = await _service.GetCheepsFromAuthor(author, page);
        foreach (var row in returnList)
        {
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email, "Follow"));
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
            Cheeps.Add(new CheepViewModel(row.Author.Name, row.Text, row.TimeStamp.ToString(), row.Author.Email, "Follow"));
        }

        return RedirectToPage("UserTimeline");
    }

    [BindProperty]
    public string Email { get; set; }

    public async Task<IActionResult> OnPostFollow([FromQuery] int page = 0)
    {
        var id = await _service.GetAuthorId(Email);
        var author = await _service.GetEmail(User.Identity.Name, page);
        var IsFollowed = false;

        var followers = await _service.GetFollowers(author.Email);
        foreach(int t in followers)
        {   
            if(id == t)
            {

                IsFollowed = true;
                break;
            }
            else
            {
                IsFollowed = false;
            }
        }

        if (!IsFollowed)
        {
            _service.AddFollowerId(author, id);
        }
        else
        {
            _service.RemoveFollowerId(author, id);
        }

        followers = await _service.GetFollowers(User.Identity.Name);

        Console.WriteLine(User.Identity.Name + "You are following these people:");

        foreach(int t in followers)
        {
            Console.WriteLine(t);
        }



        return RedirectToPage("UserTimeline");
    } 

}
