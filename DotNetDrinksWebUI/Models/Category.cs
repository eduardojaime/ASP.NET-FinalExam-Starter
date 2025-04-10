using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetDrinksWebUI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // navigation property to child Product objects
        public List<Product>? Products { get; set; }
    }
}
