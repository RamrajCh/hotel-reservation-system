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
        [RoomNumber()]
        [Display(Name = "Room Number")]
        public int RoomNumber { get; set; }

        [Required]
        [Display(Name = "Room Type")]
        public int RoomTypeId { get; set; }

        public virtual RoomType RoomType { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Price per night")]
        public decimal PricePerNight { get; set; }
        
        [Required]
        [Display(Name = "Floor")]
        public int Floor { get; set; }

        [Required]
        [Display(Name = "Number of beds")]
        public int NumberOfBeds { get; set; }
    }

    // Custom validators for room number to be unique and not 0
    public class RoomNumberAttribute : ValidationAttribute {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var room = (Room) validationContext.ObjectInstance;
            if (room.RoomNumber == 0) {
                return new ValidationResult("Room number cannot be 0");
            }
            return ValidationResult.Success;
        }
    }
}