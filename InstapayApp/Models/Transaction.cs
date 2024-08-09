using InstapayApp.Models;

public class Transaction
{
    public int Id { get; set; }
    public int FromCustomerId { get; set; }
    public Customer FromCustomer { get; set; }
    public int ToCustomerId { get; set; }
    public Customer ToCustomer { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
