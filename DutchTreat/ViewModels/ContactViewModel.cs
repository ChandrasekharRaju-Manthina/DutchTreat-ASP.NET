using System;
using System.ComponentModel.DataAnnotations;

namespace DutchTreat.ViewModels
{
    public class ContactViewModel
    {
        public ContactViewModel()
        {

        }

        [Required]
        [MinLength(5, ErrorMessage = "Need miniumu 5 characters")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "Too Long ")]
        public string Message { get; set; }
    }
}
