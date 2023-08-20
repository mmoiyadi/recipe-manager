using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Data
{
    public interface IRecipeRepository
    {
        IEnumerable<Recipe> GetRecipes();
        void SaveRecipe(Recipe recipe);

        void SaveRecipes(IEnumerable<Recipe> recipes);
    }
}
