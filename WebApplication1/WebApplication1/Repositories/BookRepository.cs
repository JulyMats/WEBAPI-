using System.Data.Common;
using System.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class BookRepository : IBookRepository
{
    public async Task<Book> GetBookGenres(int idBook)
    {
        await using SqlConnection con = new SqlConnection("Server=db-mssql;Database=2019SBD;Integrated Security=True;TrustServerCertificate=True");
        await using SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        
        await con.OpenAsync();

        
        DbTransaction tran = await con.BeginTransactionAsync();
        cmd.Transaction = (SqlTransaction)tran;

        try
        {
            await checkIfBookExists(idBook);
            var genres = new List<string>();
            
            cmd.CommandText = "SELECT PK, title FROM Books WHERE PK = @IdBook";
            cmd.Parameters.AddWithValue("@IdBook", idBook);

            var reader = await cmd.ExecuteReaderAsync();
            if (!reader.Read()) return null;
            
            string title = reader["title"].ToString();
            
            cmd.Parameters.Clear();
            await reader.CloseAsync();

            
            
            bool isBookHasGenres = await checkIfBookHasAnyGenres(idBook);

            if (isBookHasGenres)
            {
                cmd.CommandText = "SELECT g.name FROM genres g INNER JOIN books_genres bg ON g.PK = bg.FK_genre WHERE bg.FK_book = @IdBook";
                cmd.Parameters.AddWithValue("@IdBook", idBook);
                var rd = await cmd.ExecuteReaderAsync();
                
                while (rd.Read()) 
                {
                    genres.Add(rd.GetString(0));
                }

                await rd.CloseAsync();
                cmd.Parameters.Clear();


            }

            var book = new Book
            {
                IdBook = idBook,
                Title = title,
                Genres = genres,
            };
            
            await tran.CommitAsync();
            
            return book;
            
            // var book = await _context.Books
            //     .Where(b => b.PK == bookId)
            //     .Select(b => new
            //     {
            //         b.PK,
            //         b.Title,
            //         Genres = _context.Genres
            //             .Where(g => g.Books.Any(bg => bg.FK_book == bookId))
            //             .Select(g => g.Name)
            //             .ToListAsync()
            //     })
            //     .FirstOrDefaultAsync();
            //
            // if (book == null)
            // {
            //     return null;
            // }
            //
            // return book.Genres;
        } catch (SqlException exc)
        {
            await tran.RollbackAsync();
            return null;
        }
    }

    public async Task<Book> AddBook(BookDTO bookDto)
    {
        await using SqlConnection con = new SqlConnection("Server=db-mssql;Database=2019SBD;Integrated Security=True;TrustServerCertificate=True");
        await using SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        
        await con.OpenAsync();
        
        DbTransaction tran = await con.BeginTransactionAsync();
        cmd.Transaction = (SqlTransaction)tran;
        try
        {
            
            cmd.CommandText = "INSERT INTO books (title) VALUES (@title); SELECT SCOPE_IDENTITY();";
            cmd.Parameters.AddWithValue("@title", bookDto.Title);
            int bookId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            cmd.Parameters.Clear();

            
            foreach (var genreId in bookDto.Genres)
            {
                cmd.CommandText = "INSERT INTO books_genres (FK_book, FK_genre) VALUES (@bookId, @genreId)";
                cmd.Parameters.AddWithValue("@bookId", bookId);
                cmd.Parameters.AddWithValue("@genreId", genreId);
                await cmd.ExecuteNonQueryAsync();
            }
            
            Console.WriteLine("Book added");
            cmd.Parameters.Clear();
            
            await tran.CommitAsync();

            return await GetBookGenres(bookId);
            
            
            // var book = new Book
            // {
            //     Title = title
            // };
            //
            // _context.Books.Add(book);
            // await _context.SaveChangesAsync();
            //
            // if (genreIds != null)
            // {
            //     foreach (var genreId in genreIds)
            //     {
            //         _context.BooksGenres.Add(new BookGenre { FK_book = book.PK, FK_genre = genreId });
            //     }
            //     await _context.SaveChangesAsync();
            // }
        
        } catch (SqlException exc)
        {
            await tran.RollbackAsync();
            return null;
        }
    }

    private async Task<bool> checkIfBookExists(int idBook)
    {
        await using SqlConnection con = new SqlConnection("Server=db-mssql;Database=2019SBD;Integrated Security=True;TrustServerCertificate=True");
        await using SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        
        await con.OpenAsync();
        
        cmd.CommandText = "SELECT COUNT(*) FROM Books WHERE PK = @IdBook";
        cmd.Parameters.AddWithValue("@IdBook", idBook);

        int bookCounter = (int)await cmd.ExecuteScalarAsync();

        if (bookCounter < 1)
        {
            throw new Exception("There is no book with such id.");
        }
        
        cmd.Parameters.Clear();

        return true;

    }
    
    private async Task<bool> checkIfBookHasAnyGenres(int idBook)
    {
        await using SqlConnection con = new SqlConnection("Server=db-mssql;Database=2019SBD;Integrated Security=True;TrustServerCertificate=True");
        await using SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        
        await con.OpenAsync();
        
        cmd.CommandText = "SELECT COUNT(*) FROM genres g INNER JOIN books_genres bg ON g.PK = bg.FK_genre WHERE bg.FK_book = @IdBook";
        cmd.Parameters.AddWithValue("@IdBook", idBook);

        int genresCounter = (int) await cmd.ExecuteScalarAsync();


        
        if (genresCounter < 1)
        {
            return false;
            throw new Exception("There is no genres for book with entered id.");
        }
        
        cmd.Parameters.Clear();

        return true;

    }
}