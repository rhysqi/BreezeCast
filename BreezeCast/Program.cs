using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;

using BreezeCast.Hubs;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddCors(options =>
// {
// 	options.AddPolicy("Rule1", policy => 
// 	{
// 		policy.AllowAnyOrigin()
// 			.AllowAnyHeader()
// 			.AllowAnyMethod()
// 			.AllowCredentials();
// 	});
// });

builder.Services.AddSignalR();

var app = builder.Build();
// app.UseCors("Rule1");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.MapGet("/", () => "✅ BreezeCast is running on Railway!");
app.MapHub<ChatHub>("/chat");

app.Run();
