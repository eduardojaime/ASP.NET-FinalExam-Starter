using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetDrinks.Models
{
    public class OrderDetail
    {
        // junction between Order and Product (many-to-many)
        public int Id { get; set; }
        
        // fk's
        public int ProductId { get; set; }
        public int OrderId { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // navigation virtual properties
        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
