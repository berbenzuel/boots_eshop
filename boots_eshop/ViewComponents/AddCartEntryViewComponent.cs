
using BootEshop.Models;
using BootEshop.ViewModels;
using boots_eshop.Models.Detail;
using DatabaseManager.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BootEshop.ViewComponents;

public class AddCartEntryViewComponent(StockService stockService) : ViewComponent
{
    private readonly StockService _stockService = stockService;

    public async Task<IViewComponentResult> InvokeAsync(Guid productId, Guid? colorId = null, Guid? sizeId = null)
    {
        var stocks = _stockService.GetStock(productId)
            .Include(s => s.Product)
            .Include(s => s.ProductSize)
            .Include(s => s.ProductColor).AsEnumerable();

        if (!stocks.Any())
        {
            var errorModel = new AddCartEntryViewModel();
            return View(errorModel);
        }
        
        
        var colors = stocks.Select(s => s.ProductColor).Distinct().ToList();
        var sizes = stocks.Select(s => s.ProductSize).Distinct().ToList();
        
        
        var model = new AddCartEntryViewModel()
        {
            ProductId = productId,
            Variants = stocks.Select(s => 
                new DetailVariant()
                {
                    Color = colors.FirstOrDefault(c => c.Id == s.ProductColorId),
                    Size = sizes.FirstOrDefault(c => c.Id == s.ProductSizeId),
                }).ToList(),

            Quantity = 1
        };
        var colorsSelect = model.Variants.Select(v => v.Color).ToList();
        
        model.SelectedColorId = colorId is not null 
                ? colorsSelect.First(c => c.Id == colorId).Id :
                colorsSelect.First().Id;
        model.SelectedSizeId = sizeId is not null
            ? sizes.First(c => c.Id == sizeId).Id
            : null;
        return View(model);
    }
}