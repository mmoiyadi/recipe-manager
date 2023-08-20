using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    public  class Recipe 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<Ingredient> Ingredients { get; set; }
        public IEnumerable<string> Instructions { get; set; }
        public Category Category { get; set; }

        
    }
}
