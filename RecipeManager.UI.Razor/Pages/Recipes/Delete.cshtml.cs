using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeManager.API.Models;
using RecipeManager.ViewModel;

namespace RecipeManager.UI.Razor.Pages.Recipes
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public string Title { get; set; }
        [BindProperty]
        public int Id { get; set; }

        public DeleteModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
                var recipe = await response?.Content.ReadFromJsonAsync<RecipeViewModel>();
                Title = recipe.Title;
                Id = recipe.Id;
                return Page();
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var deleteRecipeUrl = _configuration.GetValue<string>("RecipeApiUrl") + $"/{id}";
            await client.DeleteAsync(deleteRecipeUrl);
            return RedirectToPage("./Index");
        }
    }
}
