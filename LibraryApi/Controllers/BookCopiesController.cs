using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookCopiesController : ControllerBase
{
    private readonly LibraryContext _context;

    public BookCopiesController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookCopy>>> GetAll()
    {
        return await _context.BookCopies
            .Include(copy => copy.Book)
            .Include(copy => copy.Branch)
            .AsNoTracking()
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookCopy>> GetById(int id)
    {
        var copy = await _context.BookCopies
            .Include(c => c.Book)
            .Include(c => c.Branch)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (copy is null)
        {
            return NotFound();
        }

        return copy;
    }

    [HttpPost]
    public async Task<ActionResult<BookCopy>> Create(BookCopy copy)
    {
        _context.BookCopies.Add(copy);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = copy.Id }, copy);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, BookCopy updated)
    {
        if (id != updated.Id)
        {
            return BadRequest();
        }

        _context.Entry(updated).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}/move/{branchId:int}")]
    public async Task<IActionResult> MoveCopy(int id, int branchId)
    {
        var copy = await _context.BookCopies.FindAsync(id);
        if (copy is null)
        {
            return NotFound();
        }

        if (!await _context.Branches.AnyAsync(branch => branch.Id == branchId))
        {
            return NotFound("Branch not found.");
        }

        copy.BranchId = branchId;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var copy = await _context.BookCopies.FindAsync(id);
        if (copy is null)
        {
            return NotFound();
        }

        _context.BookCopies.Remove(copy);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
