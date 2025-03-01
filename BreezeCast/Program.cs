using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;

using BreezeCast.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true));
});

builder.Services.AddSignalR();

var app = builder.Build();
app.UseCors();
app.MapHub<ChatHub>("/chat");

app.Run();
