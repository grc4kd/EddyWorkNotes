using DataEntities;
using Microsoft.EntityFrameworkCore;

namespace ui.Data
{
    public class EddyWorkNotesContext : DbContext
    {
        public EddyWorkNotesContext(DbContextOptions<EddyWorkNotesContext> options)
            : base(options)
        {
        }

        public DbSet<WorkNote> WorkNote { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<WorkNote>()
                .Property(e => e.RecordedAtTime)
                .HasDefaultValueSql("now()");
        }
    }
}
