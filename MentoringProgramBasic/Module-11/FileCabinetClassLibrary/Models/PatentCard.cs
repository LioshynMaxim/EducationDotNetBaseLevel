namespace FileCabinetClassLibrary.Models;

public class PatentCard : BaseCard
{
    public string Title { get; set; }
    public List<string> Authors { get; set; }
    public DateTime DatePublished { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string UniqueId { get; set; }
}
