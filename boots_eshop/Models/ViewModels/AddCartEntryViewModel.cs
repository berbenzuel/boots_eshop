using BootEshop.Models;
using boots_eshop.Models.Detail;
using Database.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BootEshop.ViewModels;

public class AddCartEntryViewModel
{
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
    public Guid? StockId { get; set; }
    public List<DetailVariant> Variants { get; set; } = new();

    public Guid? SelectedColorId { get; set; }
    public Guid? SelectedSizeId { get; set; }

}