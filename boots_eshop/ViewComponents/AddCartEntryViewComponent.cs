
using BootEshop.Models;
using BootEshop.ViewModels;
using DatabaseManager.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BootEshop.ViewComponents;

public class AddCartEntryViewComponent(StockService stockService) : ViewComponent
{
    private readonly StockService _stockService = stockService;

    public async Task<IViewComponentResult> InvokeAsync(Guid productId, Guid? colorId = null)
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
        
        
        var colors = stocks.Select(s => s.ProductColor).Distinct().Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList();
        var sizes = stocks.Select(s => s.ProductSize).Distinct().Select(s => new SelectListItem(s.Size.ToString(), s.Id.ToString())).ToList();
        
        
        var model = new AddCartEntryViewModel()
        {
            ProductId = productId,
            Variants = stocks.Select(s => 
                new Variant()
                {
                    Color = colors.FirstOrDefault(c => c.Value == s.ProductColorId.ToString()),
                    Size = sizes.FirstOrDefault(c => c.Value == s.ProductSizeId.ToString()),
                    stockId = s.Id
                }).ToList(),

            Quantity = 1
        };
        var colorsSelect = model.Variants.Select(v => v.Color).ToList();
        
        model.SelectedColorId = colorId is not null 
                ? colorsSelect.First(c => c.Value == colorId.ToString()).Value :
                colorsSelect.First().Value;
        return View(model);
    }
}