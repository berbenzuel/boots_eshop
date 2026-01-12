namespace boots_eshop.Models.ViewModels;

public class CartItemViewModel
{
    public string ProductName  { get; set; }
    public Guid ProductId  { get; set; }
    public Guid StockId { get; set; }
    public int Quantity { get; set; } = 0;
    public int Size { get; set; }
    public Guid SizeId { get; set; }
    public string Color { get; set; }
    public Guid ColorId { get; set; }
    public double UnitPrice { get; set; }
    public double? SaleUnitPrice {get; set;}
    
    
    public double TotalPrice {get => SaleUnitPrice is { } salePrice ? Quantity * salePrice : Quantity * UnitPrice;}
}