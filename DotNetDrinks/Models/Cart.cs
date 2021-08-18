using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetDrinks.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        // how can I identify an anonymous user? Random GUID
        // where to store that GUID so that we can identify browsers? Session 
        public string CustomerId { get; set; } 
        public decimal Price { get; set; }
        public DateTime DateCreated { get; set; }

        // parent nav property
        public Product Product { get; set; }
    }
}
