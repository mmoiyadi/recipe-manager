﻿namespace RecipeManager.API.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<Ingredient> Ingredients { get; set; }
        public IEnumerable<string> Instructions { get; set; }
        public int Category { get; set; }


    }
}
