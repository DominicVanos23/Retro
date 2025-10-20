using Microsoft.AspNetCore.Http;

namespace MajokoRentals.Models
{
    public class ApplicationModel
    {
        public int ApplicationID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CellNumber { get; set; }
        public string EmploymentDetails { get; set; }
        public string RentalHistory { get; set; }

        public IFormFile CertifiedID { get; set; }
        public IFormFile ProofOfIncome { get; set; }
        public IFormFile ReferencesDoc { get; set; }

        // Optional: store file paths
        public string CertifiedIDPath { get; set; }
        public string ProofOfIncomePath { get; set; }
        public string ReferencesDocPath { get; set; }

        public DateTime SubmissionDate { get; set; }
        public string Status { get; set; } = "Pending"; // For admin review
    }
}
