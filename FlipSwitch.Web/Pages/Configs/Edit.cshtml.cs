using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FlipSwitch.Web.Data;

namespace FlipSwitch.Web.Pages.Configs
{
    public class EditModel(FlipDbContext context) : PageModel
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
                Value = bool.Parse(config.Value)
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

            config.Value = this.Config.Value.ToString();
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
            public bool Value { get; set; }
        }
    }
}
