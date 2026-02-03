using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly LibraryContext _context;

    public MembersController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Member>>> GetAll()
    {
        return await _context.Members.AsNoTracking().ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Member>> GetById(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member is null)
        {
            return NotFound();
        }

        return member;
    }

    [HttpPost]
    public async Task<ActionResult<Member>> Create(Member member)
    {
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = member.Id }, member);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Member updated)
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
        var member = await _context.Members.FindAsync(id);
        if (member is null)
        {
            return NotFound();
        }

        _context.Members.Remove(member);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
