using System.Text.Json;
using BootEshop.Models;
using BootEshop.ViewModels;
using boots_eshop.Models.ViewModels;
using DatabaseManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using ZstdSharp.Unsafe;

namespace BootEshop.Controllers;

public class CartController(StockService service) : Controller
{
    
    private readonly StockService _stockService = service;
    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetString("Cart") is string str ? JsonSerializer.Deserialize<Cart>(str) : new Cart();
        var stock = _stockService.GetEntities()
            .Include(s => s.Product)
            .Include(s => s.ProductColor)
            .Include(s => s.ProductSize);
        var model = new CartViewModel()
        {
            CartItems = cart.CartEntries.Select(c =>
            {
                var stockEntry = stock.First(s => s.Id == c.StockId);
                return new CartItemViewModel()
                {
                    ProductName = stockEntry.Product.Name,
                    Color = stockEntry.ProductColor.Name,
                    Size = stockEntry.ProductSize.Size,
                    Quantity = c.Quantity,
                    UnitPrice = stockEntry.Product.Price,
                    SaleUnitPrice = stockEntry.Product.SalePrice,
                    StockId = stockEntry.Id
                };
            }).ToList()
        };
        
        return View(model);
    }

    
    public IActionResult Add(Guid productId, Guid? colorId = null)
    {
        return ViewComponent("AddCartEntry", new  {productId, colorId});
    }

    [HttpPost]
    public IActionResult Add(AddCartEntryViewModel model)
    {
        // get or create cart
        var cart = HttpContext.Session.GetString("Cart") is {} arr
            ? JsonSerializer.Deserialize<Cart>(arr)
            : new Cart();

        // find stock id
        var id = _stockService.GetStock(model.ProductId)
            .First(s =>
                s.ProductColorId.ToString() == model.SelectedColorId &&
                s.ProductSizeId.ToString() == model.SelectedSizeId
            ).Id;


        if (cart.CartEntries.Find(c => c.StockId == model.StockId) is { } entry)
        {
            entry.Quantity += model.Quantity;
        }
        else
        {
            cart.CartEntries.Add(new  CartEntry()
            {
                Quantity = model.Quantity,
                StockId = id,
            });
    
        }
        
        var toSerialize = JsonSerializer.Serialize(cart);
        // save to session
        HttpContext.Session.SetString("Cart", toSerialize);

        return RedirectToAction("Index", "Cart");
    }
    
    [HttpPost]
    public IActionResult UpdateQuantity(Guid stockId, int quantity)
    {
        var cart = JsonSerializer.Deserialize<Cart>(HttpContext.Session.GetString("Cart"));
        
        cart.CartEntries.Find(c => c.StockId == stockId).Quantity = quantity;
        var toSerialize = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString("Cart", toSerialize);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Remove(Guid stockId)
    {
        var cart = JsonSerializer.Deserialize<Cart>(HttpContext.Session.GetString("Cart"));
        
        cart.CartEntries.RemoveAll(c => c.StockId == stockId);
        var toSerialize = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString("Cart", toSerialize);
        return RedirectToAction(nameof(Index));
    }
}