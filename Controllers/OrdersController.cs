using FoodOrderSystem_.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FoodOrderSystem_.Controllers
{
    public class OrdersController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>

        [HttpPost]
        public string Post(Order orders)
        {
            string response = string.Empty;

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString))
            {
                con.Open();

                // Start a transaction to ensure data consistency
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Check if the cart is open
                        if (IsCartOpen(orders.cartId, con, transaction))
                        {
                            // Step 2: Calculate the total amount
                            decimal totalAmount = CalculateTotalAmount(orders.cartId, con, transaction);

                            // Step 3: Insert a record into the orders table
                            string orderQuery = "INSERT INTO orders (cartID, OrderTime, TotalAmount, custName, noTel, address, status) VALUES (@cartId, @OrderTime, @TotalAmount, @custName, @noTel, @address, @Status); SELECT SCOPE_IDENTITY();";
                            SqlCommand orderCmd = new SqlCommand(orderQuery, con, transaction);
                            orderCmd.Parameters.AddWithValue("@cartId", orders.cartId);
                            orderCmd.Parameters.AddWithValue("@OrderTime", DateTime.Now);
                            orderCmd.Parameters.AddWithValue("@custName", orders.custName);
                            orderCmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                            orderCmd.Parameters.AddWithValue("@noTel", orders.noTel);
                            orderCmd.Parameters.AddWithValue("@address", orders.address);
                            orderCmd.Parameters.AddWithValue("@Status", "accept"); // Assuming initial status is "accept"

                            // Retrieve the newly generated orderID
                            int orderID = Convert.ToInt32(orderCmd.ExecuteScalar());

                            if (orderID >= 0)
                            {
                                // Step 4: Update cart status to closed
                                UpdateCartStatus(orders.cartId, con, transaction, "closed");

                                // Step 5: Insert records into the ordersFood table using orderID
                                string orderFoodQuery = "INSERT INTO ordersFood (orderID, Quantity, SubTotal, FoodID) VALUES (@OrderID, @Quantity, @Subtotal, @FoodID)";
                                SqlCommand orderFoodCmd = new SqlCommand(orderFoodQuery, con, transaction);

                                foreach (OrderFood orderFood in orders.OrderFoods)
                                {
                                    // Calculate the subtotal
                                    decimal subtotal = GetFoodPriceFromDatabase(orderFood.FoodID) * orderFood.Quantity;

                                    orderFoodCmd.Parameters.Clear();
                                    orderFoodCmd.Parameters.AddWithValue("@OrderID", orderID);
                                    orderFoodCmd.Parameters.AddWithValue("@Quantity", orderFood.Quantity);
                                    orderFoodCmd.Parameters.AddWithValue("@Subtotal", subtotal);
                                    orderFoodCmd.Parameters.AddWithValue("@FoodID", orderFood.FoodID);
                                    orderFoodCmd.ExecuteNonQuery();
                                }

                                // Commit the transaction if everything is successful
                                transaction.Commit();
                                response = "Order Placed Successfully";
                            }
                            else
                            {
                                response = "Failed to Place Order";
                            }
                        }
                        else
                        {
                            response = "Cart is already check out for orders.";
                        }
                    }
                    catch (Exception ex)
                    {
                        // Roll back the transaction in case of an error
                        transaction.Rollback();
                        response = "An error occurred: " + ex.Message;
                    }
                }
            }

            return response;
        }

        bool IsCartOpen(int cartID, SqlConnection con, SqlTransaction transaction)
        {
            // Check if the cart status is "open"
            string query = "SELECT status FROM cart WHERE cartID = @cartId";
            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                cmd.Parameters.AddWithValue("@cartId", cartID);
                var result = cmd.ExecuteScalar();
                return (result != null && result.ToString() == "open");
            }
        }

        void UpdateCartStatus(int cartID, SqlConnection con, SqlTransaction transaction, string newStatus)
        {
            // Update the cart status to the new status
            string updateQuery = "UPDATE cart SET status = @newStatus WHERE cartID = @cartId";
            using (SqlCommand cmd = new SqlCommand(updateQuery, con, transaction))
            {
                cmd.Parameters.AddWithValue("@cartId", cartID);
                cmd.Parameters.AddWithValue("@newStatus", newStatus);
                cmd.ExecuteNonQuery();
            }
        }

        decimal CalculateTotalAmount(int cartID, SqlConnection con, SqlTransaction transaction)
        {
            decimal totalAmount = 0;

            string query = "SELECT SUM(SubTotal) FROM ordersFood WHERE orderID IN (SELECT orderID FROM orders WHERE cartID = @cartId)"; // Change @CartID to @cartId
            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                cmd.Parameters.AddWithValue("@cartId", cartID); // Change @CartID to @cartId

                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    totalAmount = Convert.ToDecimal(result);
                }
            }

            return totalAmount;
        }


        // Define a function to get the food price from the database
        decimal GetFoodPriceFromDatabase(int foodID)
        {
            decimal foodPrice = 0; // Initialize with a default value, such as 0

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString))
            {
                con.Open();

                // Define a SQL query to retrieve the food price based on the foodID
                string query = "SELECT FoodPrice FROM Food WHERE FoodID = @FoodID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FoodID", foodID);

                // Execute the query to fetch the food price
                var result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    foodPrice = Convert.ToDecimal(result);
                }
            }

            return foodPrice;
        }


        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}