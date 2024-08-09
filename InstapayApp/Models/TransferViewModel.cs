using System.ComponentModel.DataAnnotations;

namespace InstapayApp.ViewModels
{
    public class TransferViewModel
    {
        [Required]
        public int FromCustomerId { get; set; }

        [Required]
        public int ToCustomerId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}
