using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MajokoRentals.Models
{
    public class ListingModel
    {
        public int ListingID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }

        public string PropertyType { get; set; } = "Apartment";
        public int Bedrooms { get; set; } = 1;
        public int Bathrooms { get; set; } = 1;
        public string Status { get; set; } = "Available";

        // Serialized image URLs stored in DB, nullable
        public string? ImageUrls { get; set; }

        // Multiple uploaded files, not mapped to DB, nullable
        [NotMapped]
        public List<IFormFile>? ImageFiles { get; set; }
    }
}
