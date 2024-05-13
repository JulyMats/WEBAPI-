using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Servicies;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]           
[ApiController]                       
public class BookController : ControllerBase
{
    private IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet("{id:int}/genres")]
    public async Task<IActionResult> GetBookGenres(int id)
    {
        var book = await _bookService.GetBookGenres(id);
        if (book==null)
        {
            return BadRequest("Book not found");
        }

        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> AddBook(BookDTO bookDto)
    {
        var book = await _bookService.AddBook(bookDto);
        return CreatedAtAction(nameof(GetBookGenres), new { id = bookDto.Title }, bookDto);
    }
    
    // [HttpGet("{id}/genres")]
    // public async Task<IActionResult> GetGenres(int id)
    // {
    //     var genres = await _repository.GetGenresForBookAsync(id);
    //     if (genres == null)
    //     {
    //         return NotFound();
    //     }
    //     return Ok(genres);
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> AddBook(BookDTO bookDTO)
    // {
    //     if (bookDTO == null || string.IsNullOrEmpty(bookDTO.Title) || bookDTO.Genres == null || bookDTO.Genres.Count == 0)
    //     {
    //         return BadRequest();
    //     }
    //
    //     await _repository.AddBookAsync(bookDTO.Title, bookDTO.Genres);
    //     return CreatedAtAction(nameof(GetGenres), new { id = bookDTO.Title }, bookDTO);
    // }
}