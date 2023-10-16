using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using RecipeManager.ViewModel;

namespace RecipeManager.UI.Razor.Pages.Recipes
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public IDictionary<int, string> CategoriesDict { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;

        }
        public IList<RecipeViewModel> Recipes { get; set; } = default;
        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient();

            var recipes = await client.GetFromJsonAsync<IEnumerable<RecipeViewModel>>(
                                        _configuration.GetValue<string>("RecipeApiUrl"));
            if(recipes == null)
            {
                recipes = new List<RecipeViewModel>();
            }
            await SetCategories();
            Recipes = recipes.OrderBy(x=>x.Id).ToList();
                                    
        }

        private async Task SetCategories()
        {
            var client = _httpClientFactory.CreateClient();
            var categories = await client.GetFromJsonAsync<IEnumerable<CategoryViewModel>>(
                _configuration.GetValue<string>("CategoriesApiUrl"));
            CategoriesDict = categories.ToDictionary(x=>x.Id, x=>x.Name);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            //await SetCategories();
            return Page();
        }


    }
}
