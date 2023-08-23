using AutoMapper;
using RecipeManager.API.Models;
using RecipeManager.ViewModel;

namespace RecipeManager.API.Profiles
{
    public class IngredientProfile : Profile
    {
        public IngredientProfile()
        {
            CreateMap<Ingredient, IngredientViewModel>().ReverseMap();
        }
    }
}
