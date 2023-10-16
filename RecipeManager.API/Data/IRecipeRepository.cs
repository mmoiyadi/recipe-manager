using RecipeManager.API.Models;

namespace RecipeManager.API.Data
{
    public interface IRecipeRepository
    {
        IEnumerable<Recipe> GetRecipes();
        void AddRecipe(Recipe recipe);

        void SaveRecipes(IEnumerable<Recipe> recipes);

        void UpdateRecipe(Recipe recipe);

        IEnumerable<Category> GetCategories();
        void AddCategory(Category category);

        void DeleteRecipe(int recipeId);
    }
}
