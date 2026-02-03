namespace LibraryApi.Models;

public class Branch
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<BookCopy> BookCopies { get; set; } = new();
}
