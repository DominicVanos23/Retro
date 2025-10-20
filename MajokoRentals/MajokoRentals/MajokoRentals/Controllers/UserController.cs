using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace MajokoRentals.Controllers
{
    public class UserController : Controller
    {
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: Manage Users
        public IActionResult ManageUsers()
        {
            int totalUsers = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Users"; // live count
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        totalUsers = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error fetching user count: " + ex.Message;
            }

            ViewBag.TotalUsers = totalUsers;
            return View();
        }

        // API endpoint for live updates
        [HttpGet]
        public IActionResult GetUserCount()
        {
            int totalUsers = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Users";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        totalUsers = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch
            {
                totalUsers = -1; // error indicator
            }

            return Json(totalUsers);
        }
    }
}
