using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreBlog.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required", AllowEmptyStrings = false)]
        [Display(Name = "Full Name")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string FullName { get; set; } = "";
        
        [Required(ErrorMessage = "Email ID is required", AllowEmptyStrings = false)]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", 
            ErrorMessage = "Invalid Email Format")]
        [Display(Name = "Email ID")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Subject is required", AllowEmptyStrings = false)]
        [DataType(DataType.Text)]
        [MaxLength(50)]
        [Display(Name = "Subject")]
        public string Subject { get; set; } = "";

        [Required(ErrorMessage = "Comment is required", AllowEmptyStrings = false)]
        [DataType(DataType.MultilineText)]
        [MaxLength(255)]
        [Display(Name = "Comments")]
        public string Comment { get; set; } = "";
        public DateTime CommentedDate { get; set; } = DateTime.Now;

    }
}
