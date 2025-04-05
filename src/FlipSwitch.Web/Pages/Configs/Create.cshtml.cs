using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlipSwitch.Web.Data.Config;
using Marten;

namespace FlipSwitch.Web.Pages.Configs
{
    public class CreateModel(IDocumentStore documentStore) : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CreateViewModel Config { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var session = documentStore.LightweightSession();
            var created = new ConfigCreated
            {
                Id = Guid.NewGuid(),
                Value = Config.Value,
                Name = Config.Name,
                Created = DateTimeOffset.UtcNow,
                LastUpdated = DateTimeOffset.UtcNow,
                Version = Guid.NewGuid().ToString()
            };
            session.Events.Append(created.Id, created);
            await session.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public class CreateViewModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
