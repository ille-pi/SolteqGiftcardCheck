using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args);

var app = host.Build();

await app.RunAsync();
