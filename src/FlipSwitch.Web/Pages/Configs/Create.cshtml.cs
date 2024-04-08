using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FlipSwitch.Web.Data;

namespace FlipSwitch.Web.Pages.Configs
{
    public class CreateModel(FlipSwitch.Web.Data.FlipDbContext context) : PageModel
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

            context.Configs.Add(new Config
            {
                Id = Guid.NewGuid().ToString(),
                Value = this.Config.Value.ToString(),
                Name = this.Config.Name,
                Created = DateTimeOffset.UtcNow,
                LastUpdated = DateTimeOffset.UtcNow,
                Version = Guid.NewGuid().ToString()
            });
            await context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public class CreateViewModel
        {
            public string Name { get; set; }
            public bool Value { get; set; }
        }
    }
}
