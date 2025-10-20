using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace MajokoRentals.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;

        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            string hashedPassword = HashPassword(Password);

            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                conn.Open();

                string sql = @"SELECT U.UserID, U.FullName, R.RoleName
                               FROM Users U
                               INNER JOIN Roles R ON U.RoleID = R.RoleID
                               WHERE U.Email = @Email AND U.PasswordHash = @PasswordHash";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string role = reader["RoleName"].ToString();
                    string fullName = reader["FullName"].ToString();

                    HttpContext.Session.SetString("UserID", reader["UserID"].ToString());
                    HttpContext.Session.SetString("FullName", fullName);
                    HttpContext.Session.SetString("Role", role);

                    if (role == "Admin" || role == "Manager")
                    {
                        return RedirectToAction("AdminBoard", "Dashboard");
                    }
                    else if (role == "Tenant")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Error = "Invalid email or password.";
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(string FullName, string Email, string Password, string ConfirmPassword, string Role)
        {
            if (Password != ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Role))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            string hashedPassword = HashPassword(Password);

            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                conn.Open();

                // Get RoleID
                int roleId = GetRoleId(Role, conn);
                if (roleId == 0)
                {
                    ViewBag.Error = "Selected role does not exist. Please contact the admin.";
                    return View();
                }

                // Check if email already exists
                string checkSql = "SELECT COUNT(*) FROM Users WHERE Email=@Email";
                SqlCommand checkCmd = new SqlCommand(checkSql, conn);
                checkCmd.Parameters.AddWithValue("@Email", Email);
                int exists = (int)checkCmd.ExecuteScalar();
                if (exists > 0)
                {
                    ViewBag.Error = "Email already registered.";
                    return View();
                }

                // Insert user
                string sql = @"INSERT INTO Users (FullName, Email, PasswordHash, RoleID) 
                       VALUES (@FullName, @Email, @PasswordHash, @RoleID)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@FullName", FullName);
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                cmd.Parameters.AddWithValue("@RoleID", roleId);

                try
                {
                    cmd.ExecuteNonQuery();
                    TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                    return RedirectToAction("Login");
                }
                catch (SqlException ex)
                {
                    ViewBag.Error = "Database error: " + ex.Message;
                    return View();
                }
            }
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AdminBoard()
        {
            if (HttpContext.Session.GetString("Role") == "Admin" || HttpContext.Session.GetString("Role") == "Manager")
                return View();
            else
                return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private int GetRoleId(string roleName, SqlConnection conn)
        {
            string sql = "SELECT RoleID FROM Roles WHERE RoleName = @RoleName";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@RoleName", roleName);

            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}
