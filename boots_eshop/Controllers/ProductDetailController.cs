using System.Runtime.CompilerServices;
using BootEshop.Models;
using BootEshop.Models.Services;
using BootEshop.Services;
using BootEshop.ViewModels;
using boots_eshop.Models.Detail;
using boots_eshop.Models.ViewModels;
using DatabaseManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BootEshop.Controllers;

public class ProductDetailController : Controller
{
    
    private readonly ProductService _productService;
    private readonly StockService _stockService;
    private readonly SourceService _sourceService;
    
    public ProductDetailController(ProductService service, StockService stockService, SourceService sourceService)
    {
        _productService = service;
        _stockService = stockService;
        _sourceService = sourceService;
    }

    
    
    public IActionResult Index(Guid productId, Guid? colorId = null, Guid? sizeId = null)
    {
        var product = _productService.GetEntities()
            .Include(p => p.Manufacturer)
            .Include(p => p.ProductCategory)
            .First(p => p.Id == productId);
        
        var stocks = _stockService.GetStock(productId)
            .Include(s => s.ProductSize)
            .Include(s => s.ProductColor).AsEnumerable();
        
        
        
        var colors = stocks.Select(s => s.ProductColor).Distinct().ToList();
        var sizes = stocks.Select(s => s.ProductSize).Distinct().ToList();
        
        
        var model = new ProductDetailViewModel()
        {
            Product = product,
            ProductCategory = product.ProductCategory,
            Manufacturer = product.Manufacturer,
            Variants = stocks.Select(s => new DetailVariant()
            {
                Color = colors.First(c => c.Id == s.ProductColorId),
                Size = sizes.First(c => c.Id == s.ProductSizeId),
                StockQuantity = s.Quantity
            }).ToList(), 
            SelectedColorId = colorId is {} color ? color : colors.First().Id,
            SelectedSizeId = sizeId,
            ImageUrls =  _sourceService.GetProductImagePaths(productId)?
                .Select(f => Url.Action("ProductImage", "Source", new {filename = Path.GetFileName(f)}))
                .ToList() ?? new List<string>(),
        };
        
        
        return View(model);
    }
    
    
}