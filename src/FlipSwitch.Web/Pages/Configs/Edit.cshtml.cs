using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FlipSwitch.Web.Data;
using ConfigDto = FlipSwitch.Common.Config;

namespace FlipSwitch.Web.Pages.Configs
{
    using Updater;

    public class EditModel(FlipDbContext context, HubUpdater updater) : PageModel
    {
        [BindProperty]
        public EditViewModel Config { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var config =  await context.Configs.FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var config =  await context.Configs.FirstOrDefaultAsync(m => m.Id == Config.Id);
            if (config == null)
            {
                return NotFound();
            }

            config.Value = this.Config.Value;
            config.LastUpdated = DateTimeOffset.UtcNow;
            config.Version = Guid.NewGuid().ToString();

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfigExists(Config.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //Needs to be a mapping. Duplicated from ConfigsEndpoint class
            var dto = new ConfigDto
            {
                Name = config.Name,
                Id = config.Id,
                Type = (Common.ConfigType)config.Type,
                Value = config.Value,
                Version = config.Version
            };
            await updater.SendConfigUpdate(dto);

            return RedirectToPage("./Index");
        }

        private bool ConfigExists(string id)
        {
            return context.Configs.Any(e => e.Id == id);
        }

        public class EditViewModel
        {
            public string Id { get; set; }
            public string? Name { get; set; }
            public string Value { get; set; }
        }
    }
}
