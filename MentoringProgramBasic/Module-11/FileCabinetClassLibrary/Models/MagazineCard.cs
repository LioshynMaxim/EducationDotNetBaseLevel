namespace FileCabinetClassLibrary.Models;

public class MagazineCard : BaseCard
{
    public string Title { get; set; }
    public string Publisher { get; set; }
    public int ReleaseNumber { get; set; }
    public DateTime PublishDate { get; set; }
}
