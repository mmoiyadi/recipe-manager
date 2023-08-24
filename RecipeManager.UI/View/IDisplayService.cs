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

        RecipeViewModel GetUpdatedRecipeFromUser(RecipeViewModel recipeViewModel, 
                                                IEnumerable<CategoryViewModel> categories);

        T Ask<T>(string message);
        void ShowSuccessMessage(string message);
        void ShowErrorMessage(string message);
        void ShowAdminMessage(string message);
        void ShowAppTitle(string message);
        void ShowExitMessage(string message);

        void ShowExceptionMessage(Exception ex);

        bool AskForMore(string message);

    }
}
