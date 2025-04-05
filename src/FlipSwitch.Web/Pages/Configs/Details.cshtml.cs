using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlipSwitch.Web.Data.Config;
using Marten;

namespace FlipSwitch.Web.Pages.Configs
{
    public class DetailsModel : PageModel
    {
        private readonly IQuerySession session;

        public DetailsModel(IQuerySession session)
        {
            this.session = session;
        }

        public Config Config { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var config = await session.LoadAsync<Config>(id);
            if (config == null)
            {
                return NotFound();
            }

            Config = config;
            return Page();
        }
    }
}
