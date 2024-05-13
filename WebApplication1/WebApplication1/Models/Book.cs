namespace WebApplication1.Models;

public class Book
{
    public int IdBook { get; set; }
    public string Title { get; set; }
    public List<string> Genres { get; set; }
}