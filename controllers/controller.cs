using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly LibraryContext _context;

    public BookController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
    {
        var books = await _context.Books.ToListAsync();
        return Ok(books);
    }
    // POST: /api/books for add book
    [HttpPost]
    public async Task<ActionResult<Book>> AddBook(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    // PUT: /api/books/{id} for update a book
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, Book book)
    {
        if (id != book.Id) return BadRequest();
        _context.Entry(book).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Books.Any(b => b.Id == id)) return NotFound();
            else throw;
        }
        return NoContent();
    }

    //DELETE: /api/books/{id} 
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound(new { message = $"Ingen bok med ID {id} hittades." });
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: /api/books/genre/{genre}
    [HttpGet("genre/{genre}")]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooksByGenre(string genre)
    {
        var books = await _context.Books
            .Where(b => b.Genre.ToLower() == genre.ToLower())
            .ToListAsync();

        if (books == null || books.Count == 0)
        {
            return NotFound(new { message = $"Inga böcker hittades i genren '{genre}'." });
        }

        return Ok(books);
    }
    
    // GET: /api/books/search?title=abc
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Book>>> SearchBooksByTitle([FromQuery]string title)
    {

        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest(new { message = "Titeln kan inte vara tom." });
        }

        var books = await _context.Books
            .Where(b => b.Title.ToLower().Contains(title.ToLower()))
            .ToListAsync();

        if (books.Count == 0)
        {
            return NotFound(new { message = $"Inga böcker hittades med titeln som innehåller '{title}'." });
        }

        return Ok(books);
    }
}



