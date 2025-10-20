namespace MajokoRentals.Models
{
    public class DashboardReport
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalArrears { get; set; }
        public int TotalProperties { get; set; }
        public int OccupiedUnits { get; set; }
        public int MaintenancePending { get; set; }
        public int MaintenanceResolved { get; set; }
    }
}
