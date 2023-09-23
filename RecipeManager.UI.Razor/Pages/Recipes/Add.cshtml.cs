using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecipeManager.ViewModel;

namespace RecipeManager.UI.Razor.Pages.Recipes
{
    public class AddModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public RecipeViewModel Recipe { get; set; } = default;

        public SelectList? Categories { get; set; }

        public AddModel(IHttpClientFactory httpClientFactory,
                        IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync()
         {
            var client = _httpClientFactory.CreateClient();
            var categories = await client.GetFromJsonAsync<IEnumerable<CategoryViewModel>>(
                _configuration.GetValue<string>("CategoriesApiUrl"));
            Categories = new SelectList(categories, 
                                nameof(CategoryViewModel.Id), 
                                nameof(CategoryViewModel.Name));
            Recipe = new RecipeViewModel(){
                Ingredients = new List<IngredientViewModel>{
                    new IngredientViewModel{
                        Name = "",
                        Quantity = 1,
                        Units = ""
                    }
                },
                Instructions = new List<string>{
                    ""
                }
            };
           return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Console.WriteLine(Recipe.Title);
            
            var client = _httpClientFactory.CreateClient();
            var createRecipeUrl = _configuration.GetValue<string>("RecipeApiUrl");
             await client.PostAsJsonAsync<RecipeViewModel>(createRecipeUrl,
                                                         Recipe);
            return RedirectToPage("./Index");
        }
    }
}
