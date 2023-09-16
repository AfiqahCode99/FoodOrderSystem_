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
    public class CartController : ApiController
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
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        [HttpPost]

        public string Post(Cart cart)
        {
            string status = "open";
            string response = string.Empty;

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString)) 
            {
                con.Open();

                // Start a transaction to ensure data consistency
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Insert a record into the cart table
                        string cartQuery = "INSERT INTO cart (status) VALUES (@Status); SELECT SCOPE_IDENTITY();";
                        SqlCommand cartCmd = new SqlCommand(cartQuery, con, transaction);
                        cartCmd.Parameters.AddWithValue("@Status", status);

                        // Retrieve the newly generated cartID
                        int cartID = Convert.ToInt32(cartCmd.ExecuteScalar());

                        if (cartID > 0)
                        {
                            // Step 2: Insert records into the cartFood table using cartID
                            string cartFoodQuery = "INSERT INTO cartFood (cartID, foodID, quantity, subtotal) VALUES (@CartID, @FoodID, @Quantity, @Subtotal)";
                            SqlCommand cartFoodCmd = new SqlCommand(cartFoodQuery, con, transaction);

                            foreach (CartFood cartFood in cart.CartFoods)
                            {
                                // Calculate the subtotal
                                decimal subtotal = GetFoodPriceFromDatabase(cartFood.FoodID) * cartFood.Quantity;

                                cartFoodCmd.Parameters.Clear();
                                cartFoodCmd.Parameters.AddWithValue("@CartID", cartID);
                                cartFoodCmd.Parameters.AddWithValue("@FoodID", cartFood.FoodID);
                                cartFoodCmd.Parameters.AddWithValue("@Quantity", cartFood.Quantity);
                                cartFoodCmd.Parameters.AddWithValue("@Subtotal", subtotal);
                                cartFoodCmd.ExecuteNonQuery();
                            }

                            // Commit the transaction if everything is successful
                            transaction.Commit();
                            response = "Cart Opened and Cart Food Items Inserted";
                        }
                        else
                        {
                            response = "No Cart Opened";
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



        // Define a function to get the food price from the database
        decimal GetFoodPriceFromDatabase(int foodID)
        {
            decimal foodPrice = 0; // Initialize with a default value, such as 0

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString)) // Replace 'connectionString' with your actual connection string
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