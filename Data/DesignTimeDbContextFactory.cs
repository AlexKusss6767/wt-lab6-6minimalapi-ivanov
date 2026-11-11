using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace wt_lab7_7postgres_efcore_ivanov.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();


        var connectionString = "Host=dpg-dbar6umq1p3s73cpi2k0-a.oregon-postgres.render.com;Port=5432;Database=wt_lab10;Username=wtlab10db_2qwo_user;Password=T2FZfFhmNqGbl5EJHlXotLyKH5vOohCi;SSL Mode=Require;Trust Server Certificate=true;";

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}