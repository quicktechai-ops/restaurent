using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Restaurant.API.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Use a default connection string for migrations
        // This should match your appsettings.json connection string
        optionsBuilder.UseNpgsql("Host=dpg-d4plm1k9c44c73atige0-a.oregon-postgres.render.com;Port=5432;Database=restaurant_pos_ag2k;Username=restaurant_admin;Password=GigLT5tCw6KZmIIX8IdSaRV9pulwF2RV;SSL Mode=Require;Trust Server Certificate=true");
        
        return new AppDbContext(optionsBuilder.Options);
    }
}
