using WebApplication1.Models;

namespace WebApplication1.Repositories;

public interface IBookRepository
{
    Task<Book> GetBookGenres(int idBook);
    Task<Book> AddBook(BookDTO bookDto);
}