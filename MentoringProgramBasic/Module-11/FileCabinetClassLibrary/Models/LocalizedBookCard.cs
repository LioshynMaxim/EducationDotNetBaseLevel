namespace FileCabinetClassLibrary.Models;

public class LocalizedBookCard : BaseCard
{
    public string ISBN { get; set; }
    public string Title { get; set; }
    public List<string> Authors { get; set; }
    public int NumberOfPages { get; set; }
    public string OriginalPublisher { get; set; }
    public string CountryOfLocalization { get; set; }
    public string LocalPublisher { get; set; }
    public DateTime DatePublished { get; set; }
}
