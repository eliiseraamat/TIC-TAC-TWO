using DAL;
using Microsoft.EntityFrameworkCore;
using Tic_tac_two2;

var connectionString = $"Data Source={FileHelper.BasePath}app.db";
        
var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connectionString)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;


using var db = new AppDbContext(contextOptions);

var configRepository = new ConfigRepositoryDb(db);
//var configRepository = new ConfigRepositoryJson();
var gameRepository = new GameRepositoryDb(db);
//var gameRepository = new GameRepositoryJson();


Menus.Init(configRepository, gameRepository);
Menus.MainMenu.Run();

