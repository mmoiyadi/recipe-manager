using AutoMapper;
using RecipeManager.API.Models;
using RecipeManager.ViewModel;

namespace RecipeManager.API.Profiles
{
    public class RecipeProfile : Profile
    {
        public RecipeProfile() 
        {
            CreateMap<Recipe, RecipeViewModel>();
            CreateMap<RecipeViewModel, Recipe>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.CategoryId));


        }
    }
}
