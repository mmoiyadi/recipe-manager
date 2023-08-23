using RecipeManager.UI.Models;
using RecipeManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.UI.View
{
    public interface IDisplayService
    {
        void DisplayRecipes(IEnumerable<RecipeViewModel> recipes);
        UserSelection GetUserSelection();

        RecipeViewModel GetRecipeFromUser(IEnumerable<CategoryViewModel> categories);

        int GetRecipeToEditFromUser();

        RecipeViewModel GetUpdatedRecipeFromUser(RecipeViewModel recipeViewModel, 
                                                IEnumerable<CategoryViewModel> categories);
    }
}
