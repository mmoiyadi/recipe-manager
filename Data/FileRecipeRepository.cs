using Microsoft.Extensions.Configuration;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecipeManager.Data
{
    public class FileRecipeRepository : IRecipeRepository
    {
        private readonly IConfiguration _configuration;
        public FileRecipeRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IEnumerable<Recipe> GetRecipes()
        {
            var recipeFilePath = _configuration.GetValue<string>("RecipeFilePath");
            using FileStream fsRecipe = File.OpenRead(recipeFilePath);
            List<Recipe> recipes = JsonSerializer.Deserialize<List<Recipe>>(fsRecipe, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            fsRecipe.Close();
            return recipes;
        }

        public void SaveRecipe(Recipe recipe)
        {
            var recipes = GetRecipes().ToList();

            int maxId = recipes.Max(x => x.Id);
            recipe.Id = maxId + 1;
            recipes.Add(recipe);

            string text = JsonSerializer.Serialize(recipes);
            File.WriteAllText(_configuration.GetValue<string>("RecipeFilePath"), text);
        }

        public void SaveRecipes(IEnumerable<Recipe> recipes)
        {
            string text = JsonSerializer.Serialize(recipes);
            File.WriteAllText(_configuration.GetValue<string>("RecipeFilePath"), text);
        }
    }
}
