namespace ui.Data
{
    public static class Extensions
    {
        public static void CreateDbIfNotExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<EddyWorkNotesContext>();
            context.Database.EnsureCreated();
        }
        
        public static void InitializeDb(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<EddyWorkNotesContext>();
            SeedData.Initialize(context);
        }
    }
}
