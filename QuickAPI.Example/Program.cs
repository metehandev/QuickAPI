using QuickAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddQuickApi(true, typeof(Program));
var app = builder.Build();
app.UseQuickApi();
await app.RunAsync();