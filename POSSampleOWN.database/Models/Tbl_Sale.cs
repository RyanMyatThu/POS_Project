namespace POSSampleOWN.database.Models;

public class Tbl_Sale
{
    public int Id { get; set; }

    public decimal TotalPrice { get; set; }

    public string VoucherCode { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public ICollection<Tbl_SaleItem> SaleItems { get; set; } = new List<Tbl_SaleItem>();

}
