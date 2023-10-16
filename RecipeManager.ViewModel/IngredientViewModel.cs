using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.ViewModel
{
    public class IngredientViewModel
    {
        [Required(ErrorMessage = "Ingredient must have a name")]
        public string Name { get; set; }
        public double Quantity { get; set; }

        [Required]
        public string Units { get; set; }


        public override string ToString()
        {
            return $"{Quantity} {Units} of {Name}";
        }
    }
}
