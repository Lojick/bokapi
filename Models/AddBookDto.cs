using System.ComponentModel.DataAnnotations;

namespace bokapi.Models;

public class AddBookDto
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title can't be more than 200 characters.")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Author is required.")]
    [MaxLength(150, ErrorMessage = "Author can't be more than 150 characters.")]
    public string Author { get; set; } = "";

    [Required(ErrorMessage = "Genre is required.")]
    [MaxLength(100, ErrorMessage = "Genre can't be more than 100 characters.")]
    public string Genre { get; set; } = "";

    [Required(ErrorMessage = "Year is required.")]
    [Range(1450, 2025, ErrorMessage = "Year must be between 1450 and 2025.")]
    public int? Year { get; set; }
}
