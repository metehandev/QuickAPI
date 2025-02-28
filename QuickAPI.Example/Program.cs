using QuickAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuickApi(typeof(Program));
var app = builder.Build();
app.UseQuickApi();
await app.RunAsync();