using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodOrderSystem_.Models
{
    public class Cart
    {
        public int Id { get; set; } // Assuming Id is the unique identifier for a cart

        public string Status { get; set; }

        public List<CartFood> CartFoods { get; set; }
    }

    public class CartFood
    {
        public int Id { get; set; } // Assuming Id is the unique identifier for a cartFood

        public int CartID { get; set; }

        public int FoodID { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }
    }
}