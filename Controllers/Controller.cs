using bokapi.Data;
using bokapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BokApi.controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly LibraryContext _context;

    public BookController(LibraryContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all books from the database.
    /// </summary>
    /// <returns>A list of all books in the database.</returns>
    [HttpGet(Name = nameof(GetAllBooks))]
    public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
    {
        var books = await _context.Books.ToListAsync();
        return Ok(books);
    }
    
    /// <summary>
    /// Retrieve book by id.
    /// </summary>
    /// <param name="id">The id of the book to retrieve.</param>
    /// <returns>The book with the specified id.</returns>
    [HttpGet("{id}", Name = nameof(GetBook))]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return Ok(book);
    }

    /// <summary>
    /// Adds a new book to the database.
    /// </summary>
    /// <param name="book">The book object containing details of the book to be added.</param>
    /// <returns>The newly created book, along with the URI of the created resource.</returns>
    [HttpPost(Name = nameof(AddBook))]
    public async Task<ActionResult<Book>> AddBook(AddBookDto book)
    {
        var addedBook = _context.Books.Add(new Book
        {
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre
        });
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(AddBook), new { id = addedBook.Entity.Id }, book);
    }

    /// <summary>
    /// Updates the details of the specified book in the database.
    /// </summary>
    /// <param name="id">The id of the book to update.</param>
    /// <param name="book">The updated book object containing new book details.</param>
    /// <returns>No content if the update is successful, or appropriate error response if the operation fails.</returns>
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

    /// <summary>
    /// Deletes a book from the database by its id.
    /// </summary>
    /// <param name="id">The id of the book to delete.</param>
    /// <returns>No content if the deletion was successful, or a not found result if the book was not found.</returns>
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

    /// <summary>
    /// Retrieves all books from the database that match the specified genre.
    /// </summary>
    /// <param name="genre">The genre of books to retrieve.</param>
    /// <returns>A list of books that match the specified genre.</returns>
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

    /// <summary>
    /// Searches books in the database by a specified title.
    /// </summary>
    /// <param name="title">The title or part of the title to search for.</param>
    /// <returns>A list of books matching the specified title or an appropriate status code if no books are found or the title is invalid.</returns>
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