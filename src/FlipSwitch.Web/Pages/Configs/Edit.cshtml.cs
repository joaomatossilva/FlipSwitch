using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlipSwitch.Web.Data.Config;
using Marten;
using ConfigDto = FlipSwitch.Common.Config;

namespace FlipSwitch.Web.Pages.Configs
{
    using Updater;

    public class EditModel(IDocumentStore store, HubUpdater updater) : PageModel
    {
        [BindProperty]
        public EditViewModel Config { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var session = store.LightweightSession();
            var config = await session.LoadAsync<Config>(id);
            if (config == null)
            {
                return NotFound();
            }
            Config = new EditViewModel
            {
                Id = config.Id,
                Name = config.Name,
                Value = config.Value
            };
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var config = new ConfigUpdated
            {
                Id = id,
                Value = Config.Value,
                LastUpdated = DateTimeOffset.UtcNow,
                Version = Guid.NewGuid().ToString()
            };
            
            var session = store.LightweightSession();
            session.Events.Append(id, config);
            
            await session.SaveChangesAsync();
        
            //Needs to be a mapping. Duplicated from ConfigsEndpoint class
            var dto = new ConfigDto
            {
                Name = Config.Name ?? string.Empty,
                Id = config.Id.ToString(),
                Value = config.Value,
                Version = config.Version
            };
            
            await updater.SendConfigUpdate(dto);
        
            return RedirectToPage("./Index");
        }

        public class EditViewModel
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public string Value { get; set; }
        }
    }
}
