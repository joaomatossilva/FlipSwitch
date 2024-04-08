using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FlipSwitch.Web.Data;

namespace FlipSwitch.Web.Pages.Configs
{
    public class DeleteModel : PageModel
    {
        private readonly FlipSwitch.Web.Data.FlipDbContext _context;

        public DeleteModel(FlipSwitch.Web.Data.FlipDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Config Config { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var config = await _context.Configs.FirstOrDefaultAsync(m => m.Id == id);

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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var config = await _context.Configs.FindAsync(id);
            if (config != null)
            {
                Config = config;
                _context.Configs.Remove(Config);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
