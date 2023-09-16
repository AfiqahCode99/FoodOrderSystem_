using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FoodOrderSystem_.Controllers
{
    public class FoodController : ApiController
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        [HttpGet]
        public string Get()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Food", con);
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return JsonConvert.SerializeObject(dt);
        }

        [HttpGet]
        public string GetByID(int id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Food WHERE FoodID = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return JsonConvert.SerializeObject(dt);
        }

    }
}
