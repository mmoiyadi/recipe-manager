// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RecipeManager;
using RecipeManager.Data;
using RecipeManager.View;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
IHostEnvironment env = builder.Environment;

builder.Configuration
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
       .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddHostedService<RecipeService>();
builder.Services.AddScoped<IRecipeRepository, FileRecipeRepository>();
builder.Services.AddScoped<IDisplayService, DisplayService>();
using IHost host = builder.Build();


await host.RunAsync();