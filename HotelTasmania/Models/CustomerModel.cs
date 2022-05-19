using System.ComponentModel.DataAnnotations;

namespace HotelTasmania.Models {
     public class Customer{
            [Key]
            public int CustomerId { get; set; }
            
            [Required]
            [MaxLength(150)]
            [Display(Name = "Customer Name")]
            public string CustomerName { get; set; }
            
            [Required]
            [MaxLength(500)]
            [Display(Name = "Address")]
            public string Address { get; set; }
            
            [Required]
            [MaxLength(50)]
            [Display(Name = "Phone Number")]
            [DataType(DataType.PhoneNumber)]
            public string PhoneNumber { get; set; }
            
            [Required]
            [MaxLength(150)]
            [Display(Name = "Email")]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [DateOfBirthValidation()]
            [Display(Name = "Date of Birth")]
            public System.DateTime DateOfBirth { get; set; }
     }

    //  Validation for DOB to be in the past
    public class DateOfBirthValidationAttribute : ValidationAttribute {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var customer = (Customer) validationContext.ObjectInstance;
            if (customer.DateOfBirth > System.DateTime.Now) {
                return new ValidationResult("Date of birth cannot be in the future");
            }
            return ValidationResult.Success;
        }
    }
}