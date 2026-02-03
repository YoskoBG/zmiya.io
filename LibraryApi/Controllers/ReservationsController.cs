using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly LibraryContext _context;

    public ReservationsController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reservation>>> GetAll()
    {
        return await _context.Reservations
            .Include(r => r.Member)
            .Include(r => r.Book)
            .AsNoTracking()
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Reservation>> GetById(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Member)
            .Include(r => r.Book)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation is null)
        {
            return NotFound();
        }

        return reservation;
    }

    [HttpPost]
    public async Task<ActionResult<Reservation>> Create(ReservationCreateRequest request)
    {
        if (!await _context.Members.AnyAsync(m => m.Id == request.MemberId))
        {
            return NotFound("Member not found.");
        }

        if (!await _context.Books.AnyAsync(b => b.Id == request.BookId))
        {
            return NotFound("Book not found.");
        }

        var reservation = new Reservation
        {
            MemberId = request.MemberId,
            BookId = request.BookId,
            ReservedAt = DateTime.UtcNow,
            Status = ReservationStatus.Active
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
    }

    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation is null)
        {
            return NotFound();
        }

        reservation.Status = ReservationStatus.Cancelled;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}/fulfill")]
    public async Task<IActionResult> Fulfill(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation is null)
        {
            return NotFound();
        }

        reservation.Status = ReservationStatus.Fulfilled;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    public record ReservationCreateRequest(int MemberId, int BookId);
}
