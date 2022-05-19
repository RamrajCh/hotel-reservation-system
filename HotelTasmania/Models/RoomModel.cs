using System.ComponentModel.DataAnnotations;

namespace HotelTasmania.Models {
    
    public class RoomType{
        [Key]
        public int RoomTypeId { get; set; }
        
        [Required]
        [MaxLength(50)]
        [Display(Name = "Room Type")]
        public string RoomTypeName { get; set; }

        public virtual ICollection<Room> Room { get; set; }
    }

    public class Room{
        [Key]
        public int RoomId { get; set; }

        [Required]
        [RoomNumberValidation()]
        [Display(Name = "Room Number")]
        public int RoomNumber { get; set; }

        [Required]
        [Display(Name = "Room Type")]
        public int RoomTypeId { get; set; }

        public virtual RoomType RoomType { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [PriceValidation()]
        [Display(Name = "Price per night")]
        public decimal PricePerNight { get; set; }
        
        [Required]
        [Display(Name = "Floor")]
        [FloorValidation()]
        public int Floor { get; set; }

        [Required]
        [Display(Name = "Number of beds")]
        [NumberOfBedsValidation()]
        public int NumberOfBeds { get; set; }
    }

    // Custom validators for room number to be unique and not 0
    public class RoomNumberValidationAttribute : ValidationAttribute {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var room = (Room) validationContext.ObjectInstance;
            if (room.RoomNumber <= 0) {
                return new ValidationResult("Room number cannot be 0 or negative");
            }
            return ValidationResult.Success;
        }
    }

    // price validator
    public class PriceValidationAttribute : ValidationAttribute {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var room = (Room) validationContext.ObjectInstance;
            if (room.PricePerNight <= 0) {
                return new ValidationResult("Price per night cannot be zero or negative");
            }
            return ValidationResult.Success;
        }
    }

    // floor validator
    public class FloorValidationAttribute : ValidationAttribute {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var room = (Room) validationContext.ObjectInstance;
            if (room.Floor <= 0) {
                return new ValidationResult("Floor cannot be zero or negative");
            }
            return ValidationResult.Success;
        }
    }

    // number of beds validator
    public class NumberOfBedsValidationAttribute : ValidationAttribute {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var room = (Room) validationContext.ObjectInstance;
            if (room.NumberOfBeds <= 0) {
                return new ValidationResult("Number of beds cannot be zero or negative");
            }
            return ValidationResult.Success;
        }
    }
}