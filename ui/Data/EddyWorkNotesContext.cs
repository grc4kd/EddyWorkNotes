using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Eddy.Models;

namespace ui.Data
{
    public class EddyWorkNotesContext : DbContext
    {
        public EddyWorkNotesContext (DbContextOptions<EddyWorkNotesContext> options)
            : base(options)
        {
        }

        public DbSet<Eddy.Models.WorkNote> WorkNote { get; set; } = default!;
    }
}
