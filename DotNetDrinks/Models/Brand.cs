using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetDrinks.Models
{
    public class Brand
    {
        // in ASP.NET, key fields should always be called either Id or {Model}Id
        public int Id { get; set; }

        [Required(ErrorMessage = "Hey You!  Don't forget me!!")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Range(1400, 2025)]
        [Display(Name = "Year Founded")]
        public int YearFounded { get; set; }

        // navigation property to child Product objects
        public List<Product> Products { get; set;  }
    }
}
