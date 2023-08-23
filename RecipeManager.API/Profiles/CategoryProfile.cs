using AutoMapper;
using RecipeManager.API.Models;
using RecipeManager.ViewModel;

namespace RecipeManager.API.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile() 
        {
            CreateMap<Category, CategoryViewModel>().ReverseMap();
        }
    }
}
