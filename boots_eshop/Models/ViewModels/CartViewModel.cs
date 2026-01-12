namespace boots_eshop.Models.ViewModels;

public class CartViewModel
{
    public List<CartItemViewModel> CartItems { get; set; } = new();
    public double TotalPrice { get => CartItems.Sum(i => i.TotalPrice); }
}