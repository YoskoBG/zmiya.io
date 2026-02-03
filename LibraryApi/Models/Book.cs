namespace LibraryApi.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public int PublishYear { get; set; }
    public int PublisherId { get; set; }
    public Publisher? Publisher { get; set; }
    public List<BookCopy> Copies { get; set; } = new();
    public List<BookAuthor> BookAuthors { get; set; } = new();
    public List<BookGenre> BookGenres { get; set; } = new();
    public List<Reservation> Reservations { get; set; } = new();
}
