using AutoMapper;
using RecipeManager.API.Data;
using RecipeManager.API.Middlewares;
using RecipeManager.API.Models;
using RecipeManager.ViewModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IRecipeRepository, FileRecipeRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<TimerMiddleware>();

app.MapGet("/categories", (IRecipeRepository repo, IMapper mapper) =>
{
    var categories = repo.GetCategories();
    var categoriesVM = mapper.Map<IEnumerable<CategoryViewModel>>(categories);
    return categoriesVM;
})
.WithName("GetCategories")
.WithDisplayName("Get All Categories")
.WithDescription("Get all the categories without any filtering")
.WithOpenApi();

app.MapPost("/categories", (IRecipeRepository repo, 
                            IMapper mapper,
                            CategoryViewModel categoryVM) =>
{
    
    var category = mapper.Map<Category>(categoryVM);
    repo.AddCategory(category);
    return;
})
.WithName("AddCategory")
.WithDisplayName("Add a category")
.WithDescription("Add the given category to the repository")
.WithOpenApi();

app.MapGet("/recipes", (IRecipeRepository repo,
                        IMapper mapper) =>
{
    var recipes = repo.GetRecipes();
    
    var categories = repo.GetCategories().ToDictionary(x => x.Id);

    var recipesVM = new List<RecipeViewModel>();//mapper.Map<IEnumerable<RecipeViewModel>>(recipes);

    foreach (var recipe in recipes)
    {
        recipesVM.Add(new RecipeViewModel
        {
            Id = recipe.Id,
            Title = recipe.Title,
            CategoryName = categories[recipe.Category].Name,
            CategoryId = categories[recipe.Category].Id,
            Ingredients = mapper.Map<IEnumerable<IngredientViewModel>>(recipe.Ingredients),
            Instructions = mapper.Map<IEnumerable<string>>(recipe.Instructions)
        });
    }
    
    return recipesVM;
})
.WithName("GetRecipes")
.WithDisplayName("Get All Recipes")
.WithDescription("Get all the recipes without any filtering")
.WithOpenApi();

app.MapGet("/recipes/{recipeId}", (IRecipeRepository repo, 
                                   int recipeId, 
                                   IMapper mapper) =>
{
    var recipes = repo.GetRecipes();
    var categories = repo.GetCategories().ToDictionary(x => x.Id);
    var recipe = recipes.Where(x => x.Id == recipeId).FirstOrDefault();
    if(recipe == null)
    {
        return Results.NotFound();
    }
    var recipeVM = mapper.Map<RecipeViewModel>(recipe);
    recipeVM.CategoryId = recipe.Category;
    recipeVM.CategoryName = categories[recipe.Category].Name;
    return Results.Ok(recipeVM);
    
})
.WithName("GetRecipe")
.WithDisplayName("Get Recipe")
.WithDescription("Get the recipes with the given Id")
.WithOpenApi();

app.MapPost("/recipes", (IRecipeRepository repo, 
                         RecipeViewModel recipeVM, 
                         IMapper mapper) =>
{
    var recipe = mapper.Map<Recipe>(recipeVM);
    repo.AddRecipe(recipe);
    return;
})
.WithName("AddRecipe")
.WithDisplayName("Add a recipe")
.WithDescription("Add the given recipe to the repository ")
.WithOpenApi();

app.MapPut("/recipes/{recipeId}",  (RecipeViewModel recipeVM, 
                                    IRecipeRepository repo,
                                    IMapper mapper) =>
{
    var recipe = mapper.Map<Recipe>(recipeVM);
    repo.UpdateRecipe(recipe);

    return Results.NoContent();
})
.WithName("UpdateRecipe")
.WithDisplayName("Update a recipe")
.WithDescription("Update the recipe with given Id")
.WithOpenApi();

app.Run();

