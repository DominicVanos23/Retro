namespace MajokoRentals.Models
{
    public class MaintenanceRequest
    {
        public int RequestID { get; set; }
        public string TenantName { get; set; }
        public string UnitNumber { get; set; }
        public string Description { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string PhotoPath { get; set; }
        public string AssignedTo { get; set; }
    }
}
