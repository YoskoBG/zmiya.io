namespace LibraryApi.Models;

public class Publisher
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public List<Book> Books { get; set; } = new();
}
