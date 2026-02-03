namespace LibraryApi.Models;

public class Payment
{
    public int Id { get; set; }
    public int LoanId { get; set; }
    public Loan? Loan { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
}

public enum PaymentMethod
{
    Cash = 0,
    Card = 1
}
