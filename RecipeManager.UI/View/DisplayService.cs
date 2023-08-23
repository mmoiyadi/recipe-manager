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
                Title = AnsiConsole.Ask<string>("Give [green]Title [/]to your recipe?"),
                Ingredients = GetIngredientsFromUser(),
                Instructions = GetInstructionsFromUser()
            };
            var categoryFromUser = GetCategoryFromUser(categories);
            recipe.CategoryId = categoryFromUser.Id;
            recipe.CategoryName = categoryFromUser.Name;
            var recipeText = JsonSerializer.Serialize(recipe);
            AnsiConsole.Markup("[green]Recipe created[/]");
            /*var json = new JsonText(recipeText);
            AnsiConsole.Write(
            new Panel(json)
                .Header($"Recipe for {recipe.Title}")
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Yellow));
            */
            return recipe;
        }

        private List<IngredientViewModel> GetIngredientsFromUser()
        {
            var ingredients = new List<IngredientViewModel>
            {
                AddIngredient("Name of the ingredient?", "Select units", "Quanity?")
            };

            while (MoreIngredients())
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
                Name = AnsiConsole.Ask<string>(titleMessage),

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

        private static bool MoreIngredients()
        {
            return AnsiConsole.Confirm("Add more ingredients?");
        }

        private static List<string> GetInstructionsFromUser()
        {
            var instructions = new List<string>
            {
                AddInstruction()
            };
            while (MoreInstructions())
            {
                instructions.Add(AddInstruction());
            }
            return instructions;
        }

        private static string AddInstruction()
        {
            return AnsiConsole.Ask<string>("Enter next step:");
        }

        private static bool MoreInstructions()
        {
            return AnsiConsole.Confirm("Add more instruction?");
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

        public int GetRecipeToEditFromUser()
        {
            return AnsiConsole.Ask<int>("Enter Id of the recipe to edit: ");
        }

        public RecipeViewModel GetUpdatedRecipeFromUser(RecipeViewModel recipe, 
                                                        IEnumerable<CategoryViewModel> categories)
        {
            string newTitle = AnsiConsole.Ask<string>("Title(press enter to accept)", recipe.Title);
            if (newTitle != recipe.Title)
            {
                AnsiConsole.WriteLine($"New Title: {newTitle}");
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
            while (MoreIngredients())
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
                    newInstruction = AddInstruction();
                }

                newInstructions.Add(newInstruction);

            }
            while (MoreInstructions())
            {
                newInstructions.Add(AddInstruction());
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
    }
}
