using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RecipeManager.Data;
using RecipeManager.Models;
using RecipeManager.View;
using Spectre.Console;
using Spectre.Console.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecipeManager
{
    
    class RecipeService : IHostedService
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IDisplayService _displayService;
        public RecipeService(IRecipeRepository recipeRepository,
                             IDisplayService displayService)
        {
            _recipeRepo = recipeRepository;
            _displayService = displayService;
        }
        
        void ListRecipes()
        {
            var recipes = _recipeRepo.GetRecipes();
            _displayService.DisplayRecipes(recipes);
        }


        void AddRecipe()
        {
            Recipe recipe = _displayService.GetRecipeFromUser();
            _recipeRepo.SaveRecipe(recipe);

        }

        void EditRecipe()
        {
            var recipes = _recipeRepo.GetRecipes();
            var updatedRecipes = _displayService.EditRecipe(recipes);
            _recipeRepo.SaveRecipes(updatedRecipes);
            
        }

        void AddCategory()
        {
            try { 
                throw new NotImplementedException("This functionality is not yet implemented. Please select another option.");
            }catch(Exception ex)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
    ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
            }
        }

        public void Run()
        {
            
            var ruleTitle = new Rule("Hello and Welcome to [underline blue]Recipe Manager[/]")
            {
                Style = Style.Parse("blue dim")
            };
            AnsiConsole.Write(ruleTitle);
            var selection = UserSelection.None;
            while(selection != UserSelection.Exit)
            {
                AnsiConsole.WriteLine();
                selection = _displayService.GetUserSelection();
                switch (selection)
                {
                    case UserSelection.ListRecipe:
                        ListRecipes();
                        break;
                    case UserSelection.AddRecipe:
                        AddRecipe();
                        break;
                    case UserSelection.EditRecipe:
                        EditRecipe();
                        break;
                    case UserSelection.AddCategory:
                        AddCategory();
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Run();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            AnsiConsole.Markup("Hope you enjoyed [underline blue]Recipe Manager[/]!");
        }
    }
}
