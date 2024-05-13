using WebApplication1.Models;

namespace WebApplication1.Servicies;

public interface IBookService
{
    Task<Book> GetBookGenres(int idBook);
    Task<Book> AddBook(BookDTO bookDto);
}