using Microsoft.AspNetCore.Mvc;
using MajokoRentals.Models;
using System.Data.SqlClient;

namespace MajokoRentals.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _connectionString;

        public ApplicationController(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult Application()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Application(ApplicationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Ensure folder exists
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "Applications");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Save files
                string idPath = null, incomePath = null, refPath = null;

                if (model.CertifiedID != null)
                {
                    idPath = Path.Combine("uploads/Applications", Path.GetFileName(model.CertifiedID.FileName));
                    using (var stream = new FileStream(Path.Combine(_env.WebRootPath, idPath), FileMode.Create))
                    {
                        model.CertifiedID.CopyTo(stream);
                    }
                }

                if (model.ProofOfIncome != null)
                {
                    incomePath = Path.Combine("uploads/Applications", Path.GetFileName(model.ProofOfIncome.FileName));
                    using (var stream = new FileStream(Path.Combine(_env.WebRootPath, incomePath), FileMode.Create))
                    {
                        model.ProofOfIncome.CopyTo(stream);
                    }
                }

                if (model.ReferencesDoc != null)
                {
                    refPath = Path.Combine("uploads/Applications", Path.GetFileName(model.ReferencesDoc.FileName));
                    using (var stream = new FileStream(Path.Combine(_env.WebRootPath, refPath), FileMode.Create))
                    {
                        model.ReferencesDoc.CopyTo(stream);
                    }
                }

                // Insert into DB
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Applications 
                        (FullName, Email, CellNumber, EmploymentDetails, RentalHistory, CertifiedID, ProofOfIncome, ReferencesDoc)
                        VALUES (@FullName, @Email, @CellNumber, @EmploymentDetails, @RentalHistory, @CertifiedID, @ProofOfIncome, @ReferencesDoc)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FullName", model.FullName);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@CellNumber", model.CellNumber);
                    cmd.Parameters.AddWithValue("@EmploymentDetails", model.EmploymentDetails);
                    cmd.Parameters.AddWithValue("@RentalHistory", model.RentalHistory);
                    cmd.Parameters.AddWithValue("@CertifiedID", (object)idPath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProofOfIncome", (object)incomePath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReferencesDoc", (object)refPath ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Success = "Application submitted successfully!";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error saving application: " + ex.Message;
            }

            return View();
        }

        [HttpGet]
        public IActionResult ReviewApplication()
        {
            var applications = new List<ApplicationModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Applications ORDER BY SubmissionDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        applications.Add(new ApplicationModel
                        {
                            ApplicationID = (int)reader["ApplicationID"],
                            FullName = reader["FullName"].ToString(),
                            Email = reader["Email"].ToString(),
                            CellNumber = reader["CellNumber"].ToString(),
                            EmploymentDetails = reader["EmploymentDetails"].ToString(),
                            RentalHistory = reader["RentalHistory"].ToString(),
                            CertifiedIDPath = reader["CertifiedID"].ToString(),
                            ProofOfIncomePath = reader["ProofOfIncome"].ToString(),
                            ReferencesDocPath = reader["ReferencesDoc"].ToString(),
                            SubmissionDate = Convert.ToDateTime(reader["SubmissionDate"]),
                            Status = reader["Status"] == DBNull.Value ? "Pending" : reader["Status"].ToString()
                        });
                    }
                }
            }

            return View(applications);
        }


        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE Applications SET Status=@Status WHERE ApplicationID=@ID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["SuccessMessage"] = "Application updated successfully!";
            return RedirectToAction("ReviewApplications");
        }

    }

}

