using RecipeManager.Models;
using Spectre.Console;
using Spectre.Console.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecipeManager.View
{
    public class DisplayService : IDisplayService
    {
        private readonly string[] Units = { "count", "tbsp","cup", "kg", "gram", "litre"};
        public void DisplayRecipes(IEnumerable<Recipe> recipes)
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
                    new Text(recipe.Category.ToString(), new Style(Color.DarkOrange)).LeftJustified()
                );
                grid.AddEmptyRow();
            }

            AnsiConsole.Write(grid);
        }

        public Recipe GetRecipeFromUser()
        {
            Recipe recipe = new Recipe();
            var title = AnsiConsole.Ask<string>("Give [green]Title [/]to your recipe?");
            recipe.Title = title;
            var ingredients = new List<Ingredient>
            {
                AddIngredient("Name of the ingredient?", "Select units", "Quanity?")
            };

            while (MoreIngredients())
            {
                ingredients.Add(AddIngredient("Name of the ingredient?", "Select units", "Quantity?"));
            }
            recipe.Ingredients = ingredients;

            var instructions = new List<string>
            {
                AddInstruction()
            };
            while (MoreInstructions())
            {
                instructions.Add(AddInstruction());
            }
            recipe.Instructions = instructions;
            
            recipe.Category = GetCategoryFromUser();
            var recipeText = JsonSerializer.Serialize(recipe);
            var json = new JsonText(recipeText);
            AnsiConsole.Write(
            new Panel(json)
                .Header($"Recipe for {recipe.Title}")
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Yellow));

            return recipe;
        }

        private Category GetCategoryFromUser()
        {
            var categories = Enum.GetValues(typeof(Category)).Cast<Category>().ToDictionary(x => x.ToString(), v => (int)v);
            var categorySelected = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Select a category")
                .PageSize(10)
                .AddChoices(categories.Select(x => x.Key).ToArray())
                .WrapAround(true));
            return (Category)categories[categorySelected]; ;
        }

        public UserSelection GetUserSelection()
        {
            //AnsiConsole.Markup("Welcome to [underline blue]Recipe Manager[/]!");
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

        private string RecipeConverter(Recipe x)
        {
            return x.Title;
        }

        private bool MoreIngredients()
        {
            return AnsiConsole.Confirm("Add more ingredients?");
        }

        private bool MoreInstructions()
        {
            return AnsiConsole.Confirm("Add more instruction?");
        }

        private Ingredient AddIngredient(string titleMessage, 
                                         string unitsMessage,
                                         string quantityMessage)
        {
            Ingredient ingredient = new Ingredient
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

        private string AddInstruction()
        {
            return AnsiConsole.Ask<string>("Enter next step:");
        }

        public IEnumerable<Recipe> EditRecipe(IEnumerable<Recipe> recipes)
        {
            var recipe = AnsiConsole.Prompt(
                new SelectionPrompt<Recipe>()
                .PageSize(10)
                .Title("Select the recipe you want to edit")
                .AddChoices(recipes.ToArray())
                .WrapAround(true)
                .UseConverter(RecipeConverter));

            AnsiConsole.WriteLine($"You selected {recipe.Title} with {recipe.Id} ");
            var recipesToUpdate = recipes.ToList();
            recipesToUpdate.Remove(recipe);
            string newTitle = AnsiConsole.Ask<string>("Title(press enter to accept)", recipe.Title);
            if(newTitle != recipe.Title)
            {
                AnsiConsole.WriteLine($"New Title: {newTitle}");
                recipe.Title = newTitle;
            }
            var newIngredients = new List<Ingredient>();
            foreach(var ingredient in recipe.Ingredients)
            {
                var newIngredient = ingredient;
                if(AnsiConsole.Confirm($"{ingredient.ToString()}. Want to change this ingredient? "))
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
                if (AnsiConsole.Confirm($"{instruction}. Want to change this instruction? "))
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

            var category = recipe.Category;
            if (AnsiConsole.Confirm($"{recipe.Category.ToString()}. Want to change this category? "))
            {
                category = GetCategoryFromUser();
            }
            recipe.Category = category;
            recipesToUpdate.Add(recipe);
            AnsiConsole.Markup("[green]Recipe updated[/]");
            return recipes;
        }
    }
}
