using DAL;
using Microsoft.EntityFrameworkCore;
using Tic_tac_two2;

/*var connectionString = $"Data Source={FileHelper.BasePath}app.db";
var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connectionString)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;

using var ctx = new AppDbContext(contextOptions);
        
ctx.Database.Migrate();

Console.WriteLine(ctx.Configurations.Count());*/

Menus.MainMenu.Run();
