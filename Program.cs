using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

var builder = Host.CreateApplicationBuilder(args);

var clientID = builder.Configuration["Solteq:ClientId"];
Console.WriteLine($"client id is loaded: {!string.IsNullOrEmpty(clientID)}");

var clientSecret = builder.Configuration["Solteq:ClientSecret"];
Console.WriteLine($"client secret is loaded: {!string.IsNullOrEmpty(clientSecret)}");

var app = builder.Build();

await app.RunAsync();
