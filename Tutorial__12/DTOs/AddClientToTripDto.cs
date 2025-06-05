using System.ComponentModel.DataAnnotations;

namespace Tutorial__12.DTOs
{
    public class AddClientToTripDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Telephone { get; set; } = string.Empty;
        
        [Required]
        public string Pesel { get; set; } = string.Empty;
        
        [Required]
        public int IdTrip { get; set; }
        
        [Required]
        public string TripName { get; set; } = string.Empty;
        
        public DateTime? PaymentDate { get; set; }
    }
}