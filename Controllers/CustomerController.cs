using AdocrudApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace AdocrudApp.Controllers
{
    public class CustomerController : Controller
    {
        private string connectionString = string.Empty;
        public CustomerController(IConfiguration config)
        {
            connectionString = config["ConnectionStrings:CrudApp"];
        }
        [HttpGet]
        public IActionResult AddCustomer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCustomer(Customer cust)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("InsertCustomer", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@firstname", cust.FirstName);
                cmd.Parameters.AddWithValue("@lastname", cust.LastName);
                int myq = cmd.ExecuteNonQuery();
                if (myq > 0)
                {
                    return RedirectToAction("List");
                }
                conn.Close();
            }
           return View(cust);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Customer cust = new Customer(); 
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetById",conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cust.FirstName = reader["Firstname"].ToString();
                    cust.LastName = reader["Lastname"].ToString();
                }

                return View(cust);
            }
        }
        [HttpPost]
        public IActionResult Edit(Customer cust)
        {
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UpdateCustomer", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", cust.Id);
                cmd.Parameters.AddWithValue("@firstname", cust.FirstName);
                cmd.Parameters.AddWithValue("@lastname", cust.LastName);
                int result = cmd.ExecuteNonQuery();
                if(result>0)
                {
                    return RedirectToAction("Index", "Home");
                }
                return View(cust);

            }
        }

        [HttpGet]
        public IActionResult List()
        {
            List<Customer> custs = new List<Customer>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SelectCustomer", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    custs.Add(new Customer
                    {
                        // Assuming "Id" is also a column in your stored procedure result
                        Id = Convert.ToInt32(reader["CustomerId"]), // Assuming Id exists in the database
                        FirstName = reader["Firstname"].ToString(),
                        LastName = reader["Lastname"].ToString()
                    });
                }
            }

            return View(custs);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DeletCustomer",conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);

                int result = cmd.ExecuteNonQuery();
                if (result> 0)
                {
                    return RedirectToAction("List");
                }
                else
                {
                    return View(null);
                }

            }
        }

    }
}
