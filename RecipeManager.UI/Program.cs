// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecipeManager.UI;
using RecipeManager.UI.View;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
IHostEnvironment env = builder.Environment;

builder.Services.AddHostedService<RecipeService>();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IDisplayService, DisplayService>();

builder.Configuration
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
       .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

using IHost host = builder.Build();


await host.RunAsync();
