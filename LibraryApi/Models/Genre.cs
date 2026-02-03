namespace LibraryApi.Models;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<BookGenre> BookGenres { get; set; } = new();
}
