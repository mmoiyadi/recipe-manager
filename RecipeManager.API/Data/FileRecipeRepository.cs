using RecipeManager.API.Models;
using System.Text.Json;

namespace RecipeManager.API.Data
{
    public class FileRecipeRepository : IRecipeRepository
    {
        private readonly IConfiguration _configuration;
        public FileRecipeRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IEnumerable<Recipe>? GetRecipes()
        {
            var recipeFilePath = _configuration.GetValue<string>("RecipeFilePath");
            using FileStream fsRecipe = File.OpenRead(recipeFilePath);
            List<Recipe>? recipes = JsonSerializer.Deserialize<List<Recipe>>(fsRecipe, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            fsRecipe.Close();
            return recipes;
        }

        public void AddRecipe(Recipe recipe)
        {
            var recipes = GetRecipes().ToList();

            
            recipe.Id = GetMax<Recipe>(recipes) + 1;
            recipes.Add(recipe);

            string text = JsonSerializer.Serialize(recipes);
            File.WriteAllText(_configuration.GetValue<string>("RecipeFilePath"), text);
        }

        private int GetMax<T>(List<T> list ) where T : Base
        {
            if(list.Count == 0)
            {
                return 0;
            }
            return list.Max(x => x.Id);
        }

        public void SaveRecipes(IEnumerable<Recipe> recipes)
        {
            string text = JsonSerializer.Serialize(recipes);
            File.WriteAllText(_configuration.GetValue<string>("RecipeFilePath"), text);
        }

        public void UpdateRecipe(Recipe recipe)
        {
            var recipes = GetRecipes().ToList();
            var repoRecipe = recipes.Where(x => x.Id == recipe.Id).FirstOrDefault();
            recipes.Remove(repoRecipe);
            recipes.Add(recipe);
            SaveRecipes(recipes);
        }

        public IEnumerable<Category> GetCategories()
        {
            var categoriesFilePath = _configuration.GetValue<string>("CategoriesFilePath");
            using FileStream fsCategories = File.OpenRead(categoriesFilePath);
            List<Category>? categories = JsonSerializer.Deserialize<List<Category>>(fsCategories, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            fsCategories.Close();
            return categories;
        }

        public void AddCategory(Category category)
        {
            var categories = GetCategories().ToList();

            
            category.Id = GetMax<Category>(categories) + 1;
            categories.Add(category);

            string text = JsonSerializer.Serialize(categories);
            File.WriteAllText(_configuration.GetValue<string>("CategoriesFilePath"), text);
        }
    }
}
