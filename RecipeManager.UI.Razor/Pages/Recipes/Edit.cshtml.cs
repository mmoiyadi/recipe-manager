using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RecipeManager.API.Models;
using RecipeManager.ViewModel;

namespace RecipeManager.UI.Razor.Pages.Recipes
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public RecipeViewModel Recipe { get; set; }

        
        public SelectList Categories { get; set; }

        [BindProperty]
        public IEnumerable<CategoryViewModel> CategoriesVM { get; set; }

        public EditModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory; 
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _httpClientFactory == null)
            {
                return NotFound();
            }
            var client = _httpClientFactory.CreateClient();
            var url = _configuration.GetValue<string>("RecipeApiUrl") + $"/{id}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Recipe = await response?.Content.ReadFromJsonAsync<RecipeViewModel>();
            }
            else
            {
                return NotFound();
            }

            CategoriesVM = await client.GetFromJsonAsync<IEnumerable<CategoryViewModel>>(
                _configuration.GetValue<string>("CategoriesApiUrl"));
            Categories = new SelectList(CategoriesVM, 
                                nameof(CategoryViewModel.Id), 
                                nameof(CategoryViewModel.Name));
            
            ViewData["units"] = new List<string> { "count", "tbsp", "cup", "kg", "gram", "litre" };
            
            return Page();
            
        }

        
        public async Task<IActionResult> OnPostRecipeAsync()
        {
            ModelState.Remove("CategoryName");
            
            if (!ModelState.IsValid)
            {
                await SetCategories();
                return Page();
            }
            Console.WriteLine(Recipe.Title);
            
            var client = _httpClientFactory.CreateClient();
            var updateRecipeUrl = _configuration.GetValue<string>("RecipeApiUrl") + $"/{Recipe.Id}";
            await client.PutAsJsonAsync<RecipeViewModel>(updateRecipeUrl,
                                                        Recipe);
            return RedirectToPage("./Index");
        }
        
        public async Task<IActionResult> OnPostIngredient()
        {
            await SetCategories();
            Recipe.Ingredients.Add(new IngredientViewModel
            {
                Name = "",
                Quantity = 0,
                Units = ""
            });
            return Page();
            
        }

        public async Task<IActionResult> OnPostDeleteIngredient(int index)
        {
            await SetCategories();
            Recipe.Ingredients.RemoveAt(index);
            return Page();

        }

        public async Task<IActionResult> OnPostDeleteInstruction(int index)
        {
            await SetCategories();
            Recipe.Instructions.RemoveAt(index);
            return Page();

        }


        public async Task<IActionResult> OnPostInstruction()
        {
            await SetCategories();
            Recipe.Instructions.Add("");
            return Page();
        }

        private async Task SetCategories() 
        {
            var client = _httpClientFactory.CreateClient();
            CategoriesVM = await client.GetFromJsonAsync<IEnumerable<CategoryViewModel>>(
                _configuration.GetValue<string>("CategoriesApiUrl"));
        }
    }
}
