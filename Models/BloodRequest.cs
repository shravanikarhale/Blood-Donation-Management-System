using System.ComponentModel.DataAnnotations;

namespace BloodDonationApp.Models
{
    public class BloodRequest
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Patient Name")]
        public string PatientName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Blood Type Required")]
        public string BloodType { get; set; } = string.Empty;

        [Required]
        public int Units { get; set; }

        [Required]
        public string Hospital { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime RequiredDate { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Fulfilled, Cancelled
    }
}
