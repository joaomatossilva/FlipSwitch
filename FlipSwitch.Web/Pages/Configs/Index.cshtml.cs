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
    public class IndexModel : PageModel
    {
        private readonly FlipSwitch.Web.Data.FlipDbContext _context;

        public IndexModel(FlipSwitch.Web.Data.FlipDbContext context)
        {
            _context = context;
        }

        public IList<Config> Config { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Config = await _context.Configs.ToListAsync();
        }
    }
}
