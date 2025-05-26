using bokapi.Models;
using Microsoft.EntityFrameworkCore;

namespace bokapi.Data;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options) { }

    public DbSet<Book> Books { get; set; }
}