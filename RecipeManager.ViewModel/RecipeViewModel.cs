using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.ViewModel
{
    public class RecipeViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<IngredientViewModel> Ingredients { get; set; }
        public IEnumerable<string> Instructions { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
    }
}
