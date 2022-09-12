using Mancala.Config;
using Mancala.Filters;
using Mancala.Domains.Game.Repository;
using Mancala.Domains.Game;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IGameService, GameService>();

builder.Services.AddScoped<IGameStateRepository, GameStateRepository>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddControllers();

builder.Services.AddMvc(o => o.Filters.Add(typeof(GlobalExceptionFilters)));

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<MancalaOptions>(builder.Configuration.GetSection("MancalaConfig"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
