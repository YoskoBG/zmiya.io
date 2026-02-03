namespace LibraryApi.Models;

public class BookCopy
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public string InventoryCode { get; set; } = string.Empty;
    public CopyStatus Status { get; set; } = CopyStatus.Available;
    public List<Loan> Loans { get; set; } = new();
}

public enum CopyStatus
{
    Available = 0,
    Loaned = 1,
    Reserved = 2
}
