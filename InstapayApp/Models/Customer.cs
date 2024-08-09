using System.ComponentModel.DataAnnotations;

namespace InstapayApp.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Balance { get; set; }
    }
}
