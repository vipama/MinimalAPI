using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyApi.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDataContexts>
{
    public AppDataContexts CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDataContexts>();

        // Use a fixed server version so design-time tooling does not require live DB auto-detection.
        optionsBuilder.UseMySql(
            "Server=localhost;Port=3306;Database=Test;User=root;Password=R@x!nter1234;",
            new MySqlServerVersion(new Version(10, 4, 32)));

        return new AppDataContexts(optionsBuilder.Options);
    }
}
