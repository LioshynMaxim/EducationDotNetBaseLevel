namespace FileCabinetClassLibrary.Models;

public class BookCard : BaseCard
{
    public string ISBN { get; set; }
    public string Title { get; set; }
    public List<string> Authors { get; set; }
    public int NumberOfPages { get; set; }
    public string Publisher { get; set; }
    public DateTime DatePublished { get; set; }
}
