using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Servicies;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Book> GetBookGenres(int idBook)
    {
        return await _bookRepository.GetBookGenres(idBook);
    }

    public async Task<Book> AddBook(BookDTO bookDto)
    {
        return await _bookRepository.AddBook(bookDto);
    }

    
}