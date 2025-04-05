using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlipSwitch.Web.Data.Config;
using Marten;

namespace FlipSwitch.Web.Pages.Configs
{
    public class DeleteModel(IDocumentStore store) : PageModel
    {
        [BindProperty]
        public Config Config { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var session = store.LightweightSession();
            var config = await session.LoadAsync<Config>(id);

            if (config == null)
            {
                return NotFound();
            }
            else
            {
                Config = config;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }
        
            var session = store.LightweightSession();
            session.Events.ArchiveStream(id);
            session.Delete<Config>(id);
            await session.SaveChangesAsync();
            
            return RedirectToPage("./Index");
        }
    }
}
