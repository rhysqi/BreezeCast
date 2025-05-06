using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;

using BreezeCast.Config;
using BreezeCast.Hubs;

var builder = WebApplication.CreateBuilder(args);

var CorsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>();

builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection("CorsSettings"));
builder.Services.AddCors(options =>
{
	options.AddPolicy("AppCors", policy => 
	{
		// if (CorsSettings?.AllowedOrigins != null)
		// {
		// 	policy.WithOrigins(CorsSettings.AllowedOrigins)
		// 		.AllowAnyHeader()
		// 		.AllowAnyMethod()
		// 		.AllowCredentials();
		// }
		policy.AllowAnyOrigin()
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials();
	});
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors("AppCors");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.MapGet("/", () => "✅ BreezeCast is running on Railway!");
app.MapHub<ChatHub>("/chat");

app.Run();
