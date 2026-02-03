namespace LibraryApi.Models;

public class Loan
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public Member? Member { get; set; }
    public int BookCopyId { get; set; }
    public BookCopy? BookCopy { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public List<Payment> Payments { get; set; } = new();
}
