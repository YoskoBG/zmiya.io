using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly LibraryContext _context;

    public BranchesController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Branch>>> GetAll()
    {
        return await _context.Branches.AsNoTracking().ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Branch>> GetById(int id)
    {
        var branch = await _context.Branches.FindAsync(id);
        if (branch is null)
        {
            return NotFound();
        }

        return branch;
    }

    [HttpPost]
    public async Task<ActionResult<Branch>> Create(Branch branch)
    {
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = branch.Id }, branch);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Branch updated)
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
        var branch = await _context.Branches.FindAsync(id);
        if (branch is null)
        {
            return NotFound();
        }

        _context.Branches.Remove(branch);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
