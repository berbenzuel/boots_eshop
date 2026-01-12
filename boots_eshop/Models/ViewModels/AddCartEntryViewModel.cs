using BootEshop.Models;
using Database.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BootEshop.ViewModels;

public class AddCartEntryViewModel
{
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
    public Guid? StockId { get; set; }
    public List<Variant> Variants { get; set; } = new();

    public string? SelectedColorId { get; set; }
    public string? SelectedSizeId { get; set; }

}