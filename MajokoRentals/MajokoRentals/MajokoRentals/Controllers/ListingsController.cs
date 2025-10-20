using MajokoRentals.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Text.Json;

namespace MajokoRentals.Controllers
{
    public class ListingsController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _connectionString;

        public ListingsController(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // GET: All listings
        public IActionResult Listings()
        {
            var listings = new List<ListingModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Listings ORDER BY ListingID DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listings.Add(new ListingModel
                        {
                            ListingID = (int)reader["ListingID"],
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]),
                            Location = reader["Location"].ToString(),
                            PropertyType = reader["PropertyType"]?.ToString() ?? "Apartment",
                            Bedrooms = reader["Bedrooms"] != DBNull.Value ? Convert.ToInt32(reader["Bedrooms"]) : 1,
                            Bathrooms = reader["Bathrooms"] != DBNull.Value ? Convert.ToInt32(reader["Bathrooms"]) : 1,
                            Status = reader["Status"]?.ToString() ?? "Available",
                            ImageUrls = reader["ImageUrls"] != DBNull.Value ? reader["ImageUrls"].ToString() : JsonSerializer.Serialize(new List<string> { "uploads/default.jpg" })
                        });
                    }
                }
            }

            return View(listings);
        }

        // GET: Add listing form
        [HttpGet]
        public IActionResult AddListing() => View();

        // POST: Add listing
        [HttpPost]
        public IActionResult AddListing(ListingModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var uploadedImages = new List<string>();
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            if (model.ImageFiles != null)
            {
                foreach (var file in model.ImageFiles)
                {
                    if (file.Length > 0)
                    {
                        string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        string filePath = Path.Combine(uploadsFolder, fileName);

                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fs);
                        }

                        uploadedImages.Add($"uploads/{fileName}");
                    }
                }
            }

            if (uploadedImages.Count == 0)
                uploadedImages.Add("uploads/default.jpg");

            model.ImageUrls = JsonSerializer.Serialize(uploadedImages);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO Listings 
                                (Title, Description, Price, Location, PropertyType, Bedrooms, Bathrooms, Status, ImageUrls)
                                VALUES 
                                (@Title, @Description, @Price, @Location, @PropertyType, @Bedrooms, @Bathrooms, @Status, @ImageUrls)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", model.Title);
                    cmd.Parameters.AddWithValue("@Description", model.Description);
                    cmd.Parameters.AddWithValue("@Price", model.Price);
                    cmd.Parameters.AddWithValue("@Location", model.Location);
                    cmd.Parameters.AddWithValue("@PropertyType", model.PropertyType ?? "Apartment");
                    cmd.Parameters.AddWithValue("@Bedrooms", model.Bedrooms);
                    cmd.Parameters.AddWithValue("@Bathrooms", model.Bathrooms);
                    cmd.Parameters.AddWithValue("@Status", model.Status ?? "Available");
                    cmd.Parameters.AddWithValue("@ImageUrls", model.ImageUrls);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["SuccessMessage"] = "Listing added successfully!";
            return RedirectToAction("Listings");
        }

        // GET: Listing details by ID
        [HttpGet]
        public IActionResult Details(int id)
        {
            ListingModel listing = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Listings WHERE ListingID = @ListingID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            listing = new ListingModel
                            {
                                ListingID = (int)reader["ListingID"],
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Location = reader["Location"].ToString(),
                                PropertyType = reader["PropertyType"]?.ToString() ?? "Apartment",
                                Bedrooms = reader["Bedrooms"] != DBNull.Value ? Convert.ToInt32(reader["Bedrooms"]) : 1,
                                Bathrooms = reader["Bathrooms"] != DBNull.Value ? Convert.ToInt32(reader["Bathrooms"]) : 1,
                                Status = reader["Status"]?.ToString() ?? "Available",
                                ImageUrls = reader["ImageUrls"] != DBNull.Value ? reader["ImageUrls"].ToString() : JsonSerializer.Serialize(new List<string> { "uploads/default.jpg" })
                            };
                        }
                    }
                }
            }

            if (listing == null)
                return NotFound();

            return View(listing);
        }

        [HttpGet]
        public IActionResult ManageListing()
        {
            var listings = new List<ListingModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Listings ORDER BY ListingID DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listings.Add(new ListingModel
                        {
                            ListingID = (int)reader["ListingID"],
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]),
                            Location = reader["Location"].ToString(),
                            PropertyType = reader["PropertyType"].ToString(),
                            Bedrooms = Convert.ToInt32(reader["Bedrooms"]),
                            Bathrooms = Convert.ToInt32(reader["Bathrooms"]),
                            Status = reader["Status"].ToString(),
                            ImageUrls = reader["ImageUrls"].ToString()
                        });
                    }
                }
            }

            return View(listings);
        }

        // GET: Edit Listing
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ListingModel listing = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Listings WHERE ListingID = @ListingID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            listing = new ListingModel
                            {
                                ListingID = (int)reader["ListingID"],
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Location = reader["Location"].ToString(),
                                PropertyType = reader["PropertyType"].ToString(),
                                Bedrooms = Convert.ToInt32(reader["Bedrooms"]),
                                Bathrooms = Convert.ToInt32(reader["Bathrooms"]),
                                Status = reader["Status"].ToString(),
                                ImageUrls = reader["ImageUrls"].ToString()
                            };
                        }
                    }
                }
            }

            if (listing == null)
                return NotFound();

            return View(listing);
        }

        // POST: Edit Listing
        [HttpPost]
        public IActionResult Edit(ListingModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"UPDATE Listings SET
                                Title=@Title,
                                Description=@Description,
                                Price=@Price,
                                Location=@Location,
                                PropertyType=@PropertyType,
                                Bedrooms=@Bedrooms,
                                Bathrooms=@Bathrooms,
                                Status=@Status
                             WHERE ListingID=@ListingID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", model.Title);
                    cmd.Parameters.AddWithValue("@Description", model.Description);
                    cmd.Parameters.AddWithValue("@Price", model.Price);
                    cmd.Parameters.AddWithValue("@Location", model.Location);
                    cmd.Parameters.AddWithValue("@PropertyType", model.PropertyType);
                    cmd.Parameters.AddWithValue("@Bedrooms", model.Bedrooms);
                    cmd.Parameters.AddWithValue("@Bathrooms", model.Bathrooms);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@ListingID", model.ListingID);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["SuccessMessage"] = "Listing updated successfully!";
            return RedirectToAction("Manage");
        }

        // GET: Delete Listing
        [HttpGet]
        public IActionResult Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Listings WHERE ListingID=@ListingID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", id);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["SuccessMessage"] = "Listing deleted successfully!";
            return RedirectToAction("Manage");
        }
    }

}
