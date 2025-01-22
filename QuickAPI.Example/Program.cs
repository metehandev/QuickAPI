using QuickAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuickApi(true, typeof(Program));
var app = builder.Build();
app.UseQuickApi();
await app.RunAsync();