using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
    public class CartItem
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; } // "Buy" or "Rent"
        public int Quantity { get; set; }// number of prod 
    }
}