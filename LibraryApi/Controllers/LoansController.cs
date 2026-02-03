using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly LibraryContext _context;

    public LoansController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Loan>>> GetAll()
    {
        return await _context.Loans
            .Include(loan => loan.Member)
            .Include(loan => loan.BookCopy)
            .ThenInclude(copy => copy.Book)
            .AsNoTracking()
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Loan>> GetById(int id)
    {
        var loan = await _context.Loans
            .Include(l => l.Member)
            .Include(l => l.BookCopy)
            .ThenInclude(copy => copy.Book)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan is null)
        {
            return NotFound();
        }

        return loan;
    }

    [HttpPost]
    public async Task<ActionResult<Loan>> Create(LoanCreateRequest request)
    {
        var copy = await _context.BookCopies.FindAsync(request.BookCopyId);
        if (copy is null)
        {
            return NotFound("Book copy not found.");
        }

        if (!await _context.Members.AnyAsync(m => m.Id == request.MemberId))
        {
            return NotFound("Member not found.");
        }

        if (copy.Status != CopyStatus.Available)
        {
            return Conflict("Book copy is not available.");
        }

        copy.Status = CopyStatus.Loaned;

        var loan = new Loan
        {
            MemberId = request.MemberId,
            BookCopyId = request.BookCopyId,
            LoanDate = DateTime.UtcNow,
            DueDate = request.DueDate ?? DateTime.UtcNow.AddDays(14)
        };

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
    }

    [HttpPut("{id:int}/return")]
    public async Task<IActionResult> ReturnLoan(int id)
    {
        var loan = await _context.Loans
            .Include(l => l.BookCopy)
            .FirstOrDefaultAsync(l => l.Id == id);
        if (loan is null)
        {
            return NotFound();
        }

        if (loan.ReturnDate is not null)
        {
            return Conflict("Loan already returned.");
        }

        loan.ReturnDate = DateTime.UtcNow;
        if (loan.BookCopy is not null)
        {
            loan.BookCopy.Status = CopyStatus.Available;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    public record LoanCreateRequest(int MemberId, int BookCopyId, DateTime? DueDate);
}
