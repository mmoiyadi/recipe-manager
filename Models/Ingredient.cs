using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    public class Ingredient
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string Units { get; set; }

        public override string ToString()
        {
            return $"{Quantity} {Units} of {Name}";
        }
    }
}
