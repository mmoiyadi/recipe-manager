using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.ViewModel
{
    public class RecipeViewModel
    {
        public int Id { get; set; }

        [Required, MinLength(5)]
        public string Title { get; set; }

        [Required, MinLength(1)]
        public IList<IngredientViewModel> Ingredients { get; set; }

        [Required, MinLength(1)]
        public IList<string> Instructions { get; set; }

        //[Bindable(false)]
        //public string CategoryName { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
