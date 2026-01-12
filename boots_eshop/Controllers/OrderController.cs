using System.Text.Json;
using BootEshop.Models;
using BootEshop.Models.Services;
using BootEshop.Services;
using boots_eshop.Models.ViewModels;
using Database.Entities;
using DatabaseManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace boots_eshop.Controllers;

public class OrderController : Controller
{
    private readonly OrderService _orderService;
    private readonly StockService _stockService;
    public OrderController(OrderService orderService, StockService stockService)
    {
        _orderService = orderService;
        _stockService = stockService;
    }
    
    
    public IActionResult OrderForm()
    {
        var cart = HttpContext.Session.GetString("Cart");
        if (cart == null)
            return RedirectToAction("Index", "Home");
        
        
        var model = new OrderFormViewModel()
        {
           
        };
        
        return View(model);
    }

    [HttpPost]
    public IActionResult OrderForm(OrderFormViewModel model)
    {
        var stocks = _stockService.GetEntities()
            .Include(s => s.Product);
        
        var cart = JsonSerializer.Deserialize<Cart>(HttpContext.Session.GetString("Cart"));
        var price = cart.CartEntries.Sum(e =>
            {
                var st = stocks.First(s => s.Id == e.StockId);
                return st.Product.SalePrice is {} salePrice ? salePrice : st.Product.Price;
            }
        );


        var order = new Order()
        {
            Id = Guid.NewGuid(),
            OrderDate = DateTime.Now,
            DeliveryDate = DateTime.Now,
            Completed = false,
            Price = price,
        };
        
        
        cart.CartEntries.ForEach(ce =>
        {
            order.OrderStocks.Add(new OrderStock()
            {
                Stock = stocks.First(s => s.Id == ce.StockId),
                Order = order,
                Count = ce.Quantity
            });
        });

        _orderService.AddEntity(order);
        return RedirectToAction("Index", "Home");
    }
}