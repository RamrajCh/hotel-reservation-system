using System.ComponentModel.DataAnnotations;

namespace HotelTasmania.Models {
     public class Reservation{
        [Key]
        public int ReservationId { get; set; }

        [Required]
        [Display(Name = "Room Number")]
        public int RoomId { get; set; }

        public virtual Room Room { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        [Required]
        [CheckInDateValidation()]
        [DataType(DataType.Date)]
        [Display(Name = "Check In Date")]
        public System.DateTime CheckInDate { get; set; }

        [Required]
        [CheckOutDateValidation()]
        [DataType(DataType.Date)]
        [Display(Name = "Check Out Date")]
        public System.DateTime CheckOutDate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
     }

    //  Validate check in date
    public class CheckInDateValidationAttribute : ValidationAttribute {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var reservation = (Reservation) validationContext.ObjectInstance;
            if (reservation.CheckInDate < System.DateTime.Today) {
                return new ValidationResult("Check in date must be today or later");
            }
            return ValidationResult.Success;
        }
    }

    //  Validate check out date must be after check in date
    public class CheckOutDateValidationAttribute : ValidationAttribute {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var reservation = (Reservation) validationContext.ObjectInstance;
            if (reservation.CheckOutDate < reservation.CheckInDate) {
                return new ValidationResult("Check out date must be after check in date");
            }
            return ValidationResult.Success;
        }
    }

}