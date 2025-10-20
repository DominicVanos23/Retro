using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MajokoRentals.Models;

namespace MajokoRentals.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public MaintenanceController(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        // Tenant view: list + submission form
        [HttpGet]
        public IActionResult MaintenanceR()
        {
            List<MaintenanceRequest> requests = new List<MaintenanceRequest>();

            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                string sql = "SELECT * FROM MaintenanceRequests ORDER BY RequestDate DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    requests.Add(new MaintenanceRequest
                    {
                        RequestID = (int)reader["RequestID"],
                        TenantName = reader["TenantName"].ToString(),
                        UnitNumber = reader["UnitNumber"].ToString(),
                        Description = reader["Description"].ToString(),
                        RequestDate = (DateTime)reader["RequestDate"],
                        Status = reader["Status"].ToString(),
                        PhotoPath = reader["PhotoPath"]?.ToString(),
                        AssignedTo = reader["AssignedTo"]?.ToString()
                    });
                }
            }

            return View(requests);
        }

        // Tenant submits request
        [HttpPost]
        public IActionResult SubmitRequest(string TenantName, string UnitNumber, string Description, IFormFile Photo)
        {
            if (string.IsNullOrWhiteSpace(TenantName) || string.IsNullOrWhiteSpace(UnitNumber) || string.IsNullOrWhiteSpace(Description))
            {
                TempData["ErrorMessage"] = "All fields are required.";
                return RedirectToAction("MaintenanceR");
            }

            string photoPath = null;
            if (Photo != null && Photo.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Photo.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Photo.CopyTo(stream);
                }
                photoPath = "/uploads/" + uniqueFileName;
            }

            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                string sql = "INSERT INTO MaintenanceRequests (TenantName, UnitNumber, Description, PhotoPath) VALUES (@TenantName, @UnitNumber, @Description, @PhotoPath)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TenantName", TenantName);
                cmd.Parameters.AddWithValue("@UnitNumber", UnitNumber);
                cmd.Parameters.AddWithValue("@Description", Description);
                cmd.Parameters.AddWithValue("@PhotoPath", (object)photoPath ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }

            TempData["SuccessMessage"] = "Maintenance request submitted successfully!";
            return RedirectToAction("MaintenanceR");
        }

            // GET: Admin view of all requests
            [HttpGet]
            public IActionResult AdminMaintenance()
            {
                List<MaintenanceRequest> requests = new List<MaintenanceRequest>();
                string connectionString = _config.GetConnectionString("DefaultConnection");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT * FROM MaintenanceRequests ORDER BY RequestDate DESC";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        requests.Add(new MaintenanceRequest
                        {
                            RequestID = (int)reader["RequestID"],
                            TenantName = reader["TenantName"].ToString(),
                            UnitNumber = reader["UnitNumber"].ToString(),
                            Description = reader["Description"].ToString(),
                            RequestDate = (DateTime)reader["RequestDate"],
                            Status = reader["Status"].ToString()
                        });
                    }
                }

                return View(requests);
            }

            // POST: Update status
            [HttpPost]
            public IActionResult UpdateStatus(int requestId, string status)
            {
                if (string.IsNullOrWhiteSpace(status))
                {
                    TempData["ErrorMessage"] = "Status cannot be empty.";
                    return RedirectToAction("AdminMaintenance");
                }

                string connectionString = _config.GetConnectionString("DefaultConnection");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE MaintenanceRequests SET Status = @Status WHERE RequestID = @RequestID";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@RequestID", requestId);
                    cmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Status updated successfully!";
                return RedirectToAction("AdminMaintenance");
            }
        }
    }


