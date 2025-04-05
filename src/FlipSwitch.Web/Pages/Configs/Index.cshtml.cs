using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlipSwitch.Web.Data;
using FlipSwitch.Web.Data.Config;
using Marten;

namespace FlipSwitch.Web.Pages.Configs
{
    public class IndexModel : PageModel
    {
        private readonly IQuerySession querySession;

        public IndexModel(IQuerySession querySession)
        {
            this.querySession = querySession;
        }

        public IReadOnlyList<Config> Config { get;set; } = [];

        public async Task OnGetAsync()
        {
            Config = await querySession.Query<Config>().ToListAsync();
        }
    }
}
