using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly LibraryContext _context;

    public PaymentsController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payment>>> GetAll()
    {
        return await _context.Payments
            .Include(p => p.Loan)
            .AsNoTracking()
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Payment>> GetById(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.Loan)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment is null)
        {
            return NotFound();
        }

        return payment;
    }

    [HttpPost]
    public async Task<ActionResult<Payment>> Create(PaymentCreateRequest request)
    {
        if (!await _context.Loans.AnyAsync(l => l.Id == request.LoanId))
        {
            return NotFound("Loan not found.");
        }

        var payment = new Payment
        {
            LoanId = request.LoanId,
            Amount = request.Amount,
            Method = request.Method,
            PaidAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
    }

    public record PaymentCreateRequest(int LoanId, decimal Amount, PaymentMethod Method);
}
