using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;

namespace MajokoRentals.Controllers
{
    public class DashboardController : Controller
    {
        private readonly string _connectionString;

        public DashboardController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public IActionResult AdminBoard()
        {
            int activeListings = 0;
            int totalApplications = 0;
            int totalUsers = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Count Active Listings
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Listings WHERE Status = 'Available'", conn))
                    {
                        activeListings = (int)cmd.ExecuteScalar();
                    }

                    // Count Applications
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Applications", conn))
                    {
                        totalApplications = (int)cmd.ExecuteScalar();
                    }

                    // Count Users
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users", conn))
                    {
                        totalUsers = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle errors gracefully
                TempData["ErrorMessage"] = $"Error loading dashboard: {ex.Message}";
            }

            // Pass KPI data to view
            ViewBag.ActiveListings = activeListings;
            ViewBag.TotalApplications = totalApplications;
            ViewBag.TotalUsers = totalUsers;

            return View();
        }
    }
}
