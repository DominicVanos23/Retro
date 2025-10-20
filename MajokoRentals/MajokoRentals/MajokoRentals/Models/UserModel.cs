namespace MajokoRentals.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // Admin, Manager, Tenant
        public bool IsActive { get; set; } = true;
    }
}
