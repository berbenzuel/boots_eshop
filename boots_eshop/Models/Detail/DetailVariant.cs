using Database.Entities;

namespace boots_eshop.Models.Detail;

public class DetailVariant
{
    public ProductColor Color { get; set; }
    public ProductSize Size { get; set; }
    public int StockQuantity { get; set; }
}