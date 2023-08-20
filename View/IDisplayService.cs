using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.View
{
    public interface IDisplayService
    {
        void DisplayRecipes(IEnumerable<Recipe> recipes);
        IEnumerable<Recipe> EditRecipe(IEnumerable<Recipe> recipes);
        Recipe GetRecipeFromUser();

        UserSelection GetUserSelection();
    }
}
