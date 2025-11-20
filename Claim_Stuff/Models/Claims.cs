using System.ComponentModel.DataAnnotations;

namespace Claim_Stuff.Models
{
    public class Claims
    {
        [Key]
        public int ClaimId { get; set; }

        [Required(ErrorMessage = "Month is required.")]
        [StringLength(30)]
        [Display(Name = "Month")]
        public string month { get; set; }

        [Required(ErrorMessage = "Hours worked is required.")]
        [Range(1, 300, ErrorMessage = "Hours must be between 1 and 300.")]
        public int hours { get; set; }

        [Required(ErrorMessage = "Hourly rate is required.")]
        [Range(1, 10000, ErrorMessage = "Hourly rate must be greater than 0.")]
        public decimal rate { get; set; }


        public string documentName { get; set; } = "DocumentFile";

        [Required(ErrorMessage = "Please upload a supporting document.")]
        [DataType(DataType.Upload)]
        public IFormFile DocumentFile { get; set; }

        public decimal totalAmount { get; set; }

        public string status { get; set; } = "Pending";
    }
}
