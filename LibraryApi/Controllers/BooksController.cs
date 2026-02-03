using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly LibraryContext _context;

    public BooksController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAll()
    {
        return await _context.Books
            .Include(book => book.Publisher)
            .Include(book => book.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(book => book.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .AsNoTracking()
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Book>> GetById(int id)
    {
        var book = await _context.Books
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book is null)
        {
            return NotFound();
        }

        return book;
    }

    [HttpPost]
    public async Task<ActionResult<Book>> Create(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Book updated)
    {
        if (id != updated.Id)
        {
            return BadRequest();
        }

        _context.Entry(updated).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:int}/authors/{authorId:int}")]
    public async Task<IActionResult> AddAuthor(int id, int authorId)
    {
        if (!await _context.Books.AnyAsync(b => b.Id == id))
        {
            return NotFound("Book not found.");
        }

        if (!await _context.Authors.AnyAsync(a => a.Id == authorId))
        {
            return NotFound("Author not found.");
        }

        var exists = await _context.BookAuthors.AnyAsync(ba => ba.BookId == id && ba.AuthorId == authorId);
        if (exists)
        {
            return Conflict("Author already assigned to book.");
        }

        _context.BookAuthors.Add(new BookAuthor { BookId = id, AuthorId = authorId });
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}/authors/{authorId:int}")]
    public async Task<IActionResult> RemoveAuthor(int id, int authorId)
    {
        var link = await _context.BookAuthors.FirstOrDefaultAsync(ba => ba.BookId == id && ba.AuthorId == authorId);
        if (link is null)
        {
            return NotFound();
        }

        _context.BookAuthors.Remove(link);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:int}/genres/{genreId:int}")]
    public async Task<IActionResult> AddGenre(int id, int genreId)
    {
        if (!await _context.Books.AnyAsync(b => b.Id == id))
        {
            return NotFound("Book not found.");
        }

        if (!await _context.Genres.AnyAsync(g => g.Id == genreId))
        {
            return NotFound("Genre not found.");
        }

        var exists = await _context.BookGenres.AnyAsync(bg => bg.BookId == id && bg.GenreId == genreId);
        if (exists)
        {
            return Conflict("Genre already assigned to book.");
        }

        _context.BookGenres.Add(new BookGenre { BookId = id, GenreId = genreId });
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}/genres/{genreId:int}")]
    public async Task<IActionResult> RemoveGenre(int id, int genreId)
    {
        var link = await _context.BookGenres.FirstOrDefaultAsync(bg => bg.BookId == id && bg.GenreId == genreId);
        if (link is null)
        {
            return NotFound();
        }

        _context.BookGenres.Remove(link);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
