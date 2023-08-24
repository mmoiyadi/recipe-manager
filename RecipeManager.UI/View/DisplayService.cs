using RecipeManager.UI.Models;
using RecipeManager.UI.View;
using RecipeManager.ViewModel;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecipeManager.UI
{
    public class DisplayService : IDisplayService
    {
        private readonly string[] Units = { "count", "tbsp", "cup", "kg", "gram", "litre" };
        public void DisplayRecipes(IEnumerable<RecipeViewModel> recipes)
        {
            if (recipes.Count() == 0)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.Markup("[bold maroon on blue italic slowblink]Nothing to show here. Start by adding some recipes of your own.[/]");
                AnsiConsole.WriteLine();
                return;
            }
            recipes = recipes.OrderBy(x => x.Id);
            var grid = new Grid();

            // Add columns 
            grid.AddColumns(5);

            grid.AddEmptyRow();
            // Add header row 
            grid.AddRow(new Text[]{
                new Text("Id", new Style(Color.DarkSeaGreen, Color.Black)).LeftJustified(),
                new Text("Title", new Style(Color.DarkViolet, Color.Black)).LeftJustified(),
                new Text("Ingredients", new Style(Color.Green, Color.Black)).LeftJustified(),
                new Text("Instructions", new Style(Color.Yellow, Color.Black)).LeftJustified(),
                new Text("Category", new Style(Color.Orange1, Color.Black)).LeftJustified()
            });
            
            foreach (var recipe in recipes)
            {
                var ingredientCol = new Grid();

                ingredientCol.AddColumn();

                foreach (var ingredient in recipe.Ingredients)
                {
                    ingredientCol.AddRow(new Text(ingredient.ToString(), new Style(Color.DarkOliveGreen1)));
                }

                var instructionCol = new Grid();

                instructionCol.AddColumn();
                foreach (var instruction in recipe.Instructions)
                {
                    instructionCol.AddRow(new Text(instruction, new Style(Color.GreenYellow)).LeftJustified());
                }

                grid.AddRow(
                    new Text(recipe.Id.ToString(), new Style(Color.SeaGreen1)).LeftJustified(),
                    new Text(recipe.Title, new Style(Color.Violet)).LeftJustified(),
                    ingredientCol,
                    instructionCol,
                    new Text(recipe.CategoryName, new Style(Color.DarkOrange)).LeftJustified()
                );
                grid.AddEmptyRow();
            }

            AnsiConsole.Write(grid);
        }

        private string OptionsConverter(UserSelection x)
        {
            string result = "Invalid selection";
            switch (x)
            {
                case UserSelection.ListRecipe:
                    result = "List all recipes";
                    break;
                case UserSelection.AddRecipe:
                    result = "Add a recipe";
                    break;
                case UserSelection.EditRecipe:
                    result = "Edit a recipe";
                    break;
                case UserSelection.AddCategory:
                    result = "Add a category";
                    break;
                case UserSelection.Exit:
                    result = "Exit";
                    break;
                default: return result;
            }
            return result;
        }

        public UserSelection GetUserSelection()
        {
            AnsiConsole.WriteLine();
            UserSelection selection = AnsiConsole.Prompt(
                new SelectionPrompt<UserSelection>()
                .Title("What would you like to do?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more optoins)[/]")
                .AddChoices(new[]
                {
                    UserSelection.ListRecipe,
                    UserSelection.AddRecipe,
                    UserSelection.EditRecipe,
                    UserSelection.AddCategory,
                    UserSelection.Exit
                })
                .WrapAround(true)
                .UseConverter(OptionsConverter));

            return selection;
        }

        public RecipeViewModel GetRecipeFromUser(IEnumerable<CategoryViewModel> categories)
        {

            RecipeViewModel recipe = new()
            {
                Title = Ask<string>("Give a title for your recipe?"),
                Ingredients = GetIngredientsFromUser(),
                Instructions = GetInstructionsFromUser()
            };
            var categoryFromUser = GetCategoryFromUser(categories);
            recipe.CategoryId = categoryFromUser.Id;
            recipe.CategoryName = categoryFromUser.Name;
            
            return recipe;
        }

        private List<IngredientViewModel> GetIngredientsFromUser()
        {
            var ingredients = new List<IngredientViewModel>
            {
                AddIngredient("Name of the ingredient?", "Select units", "Quanity?")
            };

            while (AskForMore("Add more ingredients?"))
            {
                ingredients.Add(AddIngredient("Name of the ingredient?", "Select units", "Quantity?"));
            }
            return ingredients;
        }

        private IngredientViewModel AddIngredient(string titleMessage,
                                         string unitsMessage,
                                         string quantityMessage)
        {
            IngredientViewModel ingredient = new()
            {
                Name = Ask<string>(titleMessage),

                Units = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title(unitsMessage)
                .PageSize(10)
                .AddChoices(Units)
                .WrapAround(true)),
                Quantity = AnsiConsole.Prompt(
                new TextPrompt<double>(quantityMessage)
                .ValidationErrorMessage("[red] That's not a valid quantity[/]")
                )
            };
            return ingredient;
        }

        public bool AskForMore(string message)
        {
            return AnsiConsole.Confirm( message );
        }


        private  List<string> GetInstructionsFromUser()
        {
            var instructions = new List<string>
            {
                Ask<string>("Enter next step:")
            };
            while (AskForMore("Add more instruction?"))
            {
                instructions.Add(Ask<string>("Enter next step:"));
            }
            return instructions;
        }


        

        private static CategoryViewModel GetCategoryFromUser(IEnumerable<CategoryViewModel> categories)
        {
            //var categories = Enum.GetValues(typeof(CategoryViewModel)).Cast<CategoryViewModel>().ToDictionary(x => x.ToString(), v => (int)v);

            var categorySelected = AnsiConsole.Prompt(new SelectionPrompt<CategoryViewModel>()
                .Title("Select a category")
                .PageSize(10)
                .AddChoices(categories)
                .UseConverter((x) => { return x.Name; })
                .WrapAround(true));
            return categorySelected;
        }

        

        public RecipeViewModel GetUpdatedRecipeFromUser(RecipeViewModel recipe, 
                                                        IEnumerable<CategoryViewModel> categories)
        {
            string newTitle = AnsiConsole.Ask<string>("Title(press enter to accept)", recipe.Title);
            if (newTitle != recipe.Title)
            {
                recipe.Title = newTitle;
            }
            var newIngredients = new List<IngredientViewModel>();
            foreach (var ingredient in recipe.Ingredients)
            {
                var newIngredient = ingredient;
                AnsiConsole.Markup($"Current Ingredient: [bold blue]{ingredient}[/]. ");
                if (AnsiConsole.Confirm("Want to change this ingredient? "))
                {
                    newIngredient = AddIngredient("Name of the ingredient?", "Select units", "Quanity?");
                }
                newIngredients.Add(newIngredient);

            }
            while (AskForMore("Add more ingredients?"))
            {
                newIngredients.Add(AddIngredient("Name of the ingredient?", "Select units", "Quantity?"));
            }
            recipe.Ingredients = newIngredients;

            var newInstructions = new List<string>();
            foreach (var instruction in recipe.Instructions)
            {
                var newInstruction = instruction;
                AnsiConsole.Markup($"Current Instruction: [bold blue]{instruction}[/]. ");
                if (AnsiConsole.Confirm("Want to change this instruction? "))
                {
                    newInstruction = Ask<string>("Enter next step:");
                }

                newInstructions.Add(newInstruction);

            }
            while (AskForMore("Add more instruction?"))
            {
                newInstructions.Add(Ask<string>("Enter next step:"));
            }
            recipe.Instructions = newInstructions;

            var category = new CategoryViewModel
            {
                Id = recipe.CategoryId,
                Name = recipe.CategoryName
            };
            AnsiConsole.Markup($"Current Category:[bold blue]{recipe.CategoryName}[/]. ");
            if (AnsiConsole.Confirm("Want to change this category? "))
            {
                category = GetCategoryFromUser(categories);
            }
            recipe.CategoryName = category.Name;
            recipe.CategoryId = category.Id;
            return recipe;
        }

        public T Ask<T>(string message)
        {
            return AnsiConsole.Ask<T>(message); 
        }

        public void ShowSuccessMessage(string message)
        {
            AnsiConsole.Markup($"[green bold]{message}[/]");
        }

        public void ShowErrorMessage(string message)
        {
            AnsiConsole.Markup($"[red]{message}[/]");
            AnsiConsole.WriteLine();
        }

        public void ShowAdminMessage(string message)
        {
            AnsiConsole.Markup($"[darkorange3_1 bold]{message}[/]");
            AnsiConsole.WriteLine();
        }

        public void ShowAppTitle(string message)
        {
            AnsiConsole.Write(new FigletText(message).Centered().Color(Color.Purple4));
        }

        public void ShowExitMessage(string message)
        {
            var ruleExit = new Rule(message)
            {
                Style = Style.Parse("invert")
            };
            AnsiConsole.Write(ruleExit);
        }
        public void ShowExceptionMessage(Exception ex) 
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
        }
    }
}
