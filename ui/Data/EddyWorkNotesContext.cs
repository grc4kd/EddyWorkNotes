using DataEntities;
using Microsoft.EntityFrameworkCore;

namespace ui.Data
{
    public class EddyWorkNotesContext(DbContextOptions<EddyWorkNotesContext> options) : DbContext(options)
    {
        public DbSet<WorkNote> WorkNote { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<WorkNote>()
                .Property(e => e.RecordedAtTimeUtc)
                .HasDefaultValueSql("now()");
        }
    }
}
