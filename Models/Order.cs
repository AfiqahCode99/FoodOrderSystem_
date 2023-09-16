using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodOrderSystem_.Models
{
    public class Order
    {
        public int Id { get; set; } // Assuming Id is the unique identifier for a cart

        public int cartId { get; set; }

        public DateTime OrderTime { get; set;}
        public decimal totalAmount { get; set; }

        public string custName { get; set; }

        public string noTel { get; set; }

        public string address { get; set; }

        public string Status { get; set; }


        public List<OrderFood> OrderFoods { get; set; }
    }

    public class OrderFood
    {
        public int Id { get; set; } // Assuming Id is the unique identifier for a cartFood

        public int orderID { get; set; }

        public int FoodID { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }
    }
}