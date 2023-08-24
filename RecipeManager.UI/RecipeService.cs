using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RecipeManager.API.Models;
using RecipeManager.UI.Models;
using RecipeManager.UI.View;
using RecipeManager.ViewModel;
using Spectre.Console;
using System.Diagnostics;
using System.Net.Http.Json;


namespace RecipeManager.UI
{
    class RecipeService : BackgroundService
    {
        private readonly IDisplayService _displayService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public RecipeService(IDisplayService displayService,
                            IHttpClientFactory httpClientFactory,
                            IConfiguration configuration)
        {
            _displayService = displayService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        private async Task Run() 
        {
            try
            {
                var ruleTitle = new Rule("[underline blue]Recipe Manager[/]")
                {
                    Style = Style.Parse("blue dim")
                };
                AnsiConsole.Write(ruleTitle);
                var selection = UserSelection.None;
                while (selection != UserSelection.Exit)
                {
                    AnsiConsole.WriteLine();
                    selection = _displayService.GetUserSelection();
                    switch (selection)
                    {
                        case UserSelection.ListRecipe:
                            await ListRecipes();
                            break;
                        case UserSelection.AddRecipe:
                            await AddRecipe();
                            break;
                        case UserSelection.EditRecipe:
                            await EditRecipe();
                            break;
                        case UserSelection.AddCategory:
                            await AddCategory();
                            break;
                        case UserSelection.Exit:
                            var ruleExit = new Rule("Hope you enjoyed [underline blue]Recipe Manager[/]. Good Bye!!!")
                            {
                                Style = Style.Parse("invert")
                            };
                            AnsiConsole.Write(ruleExit);
                            Environment.Exit(0);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
                                            ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
            }
            
        }

        private async Task ListRecipes()
        {
            
            var client = _httpClientFactory.CreateClient();
            
            var recipes = await client.GetFromJsonAsync<IEnumerable<RecipeViewModel>>(
                _configuration.GetValue<string>("RecipeApiUrl"));
            
            _displayService.DisplayRecipes(recipes);
        }

        private async Task<IEnumerable<CategoryViewModel>> GetCategories()
        {
            var client = _httpClientFactory.CreateClient();
            return await client.GetFromJsonAsync<IEnumerable<CategoryViewModel>>(_configuration.GetValue<string>("CategoriesApiUrl"));
        }

        private async Task AddRecipe()
        {
            var client = _httpClientFactory.CreateClient();
            var categories = await GetCategories();
            RecipeViewModel recipe = _displayService.GetRecipeFromUser(categories);
            
            await client.PostAsJsonAsync<RecipeViewModel>(
                _configuration.GetValue<string>("RecipeApiUrl"),
                recipe);
        }

        private async Task EditRecipe()
        {
            var recipeId = _displayService.GetRecipeToEditFromUser();
            var client = _httpClientFactory.CreateClient();
            var url = _configuration.GetValue<string>("RecipeApiUrl") + $"/{recipeId}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var recipe = await response.Content.ReadFromJsonAsync<RecipeViewModel>();
                Console.WriteLine(recipe.Title);
                var categories = await GetCategories();
                var updatedRecipe = _displayService.GetUpdatedRecipeFromUser(recipe, categories);
                var updateResponse = await client.PutAsJsonAsync<RecipeViewModel>(_configuration.GetValue<string>("RecipeApiUrl") +$"/{updatedRecipe.Id}",
                    updatedRecipe);
                if (updateResponse.IsSuccessStatusCode)
                {
                    AnsiConsole.Markup($"[green bold]Updated {updatedRecipe.Title}[/]");
                }
                else
                {
                    AnsiConsole.Markup("[red]Failed to update the recipe[/]");
                }
            }
            else
            {
                if(response.StatusCode == System.Net.HttpStatusCode.NotFound) 
                {
                    AnsiConsole.Markup($"[red bold]Recipe not found with Id: {recipeId}[/]");
                }
                else
                {
                    AnsiConsole.Markup("[red bold]Server Error: Please try after sometime[/]");
                }
            }
        }

        private async Task AddCategory()
        {
            AnsiConsole.Markup("[darkorange3_1 bold] Note: This is an admin action[/]");
            AnsiConsole.WriteLine();
            var categoryName = AnsiConsole.Ask<string>("Enter the category you want to add?");
            var category = new CategoryViewModel
            {
                Name = categoryName
            };
            var client =_httpClientFactory.CreateClient();
            await client.PostAsJsonAsync<CategoryViewModel>(
                _configuration.GetValue<string>("CategoriesApiUrl"),
                category);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Run();
                await Task.Delay(2000, stoppingToken);
            }
            
        }
    }
}
