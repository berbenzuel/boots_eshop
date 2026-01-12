using BootEshop.Models;
using boots_eshop.Models.Detail;
using Database.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace boots_eshop.Models.ViewModels;

public class ProductDetailViewModel
{
    public Product Product { get; set; }
    public ProductCategory ProductCategory { get; set; }
    public Manufacturer Manufacturer { get; set; }
    public List<DetailVariant> Variants { get; set; } = new();

    public Guid? SelectedColorId { get; set; }
    public Guid? SelectedSizeId { get; set; }

    
    public List<string> ImageUrls { get; set; } = new();
    
}