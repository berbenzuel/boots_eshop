using Database.Entities;

namespace BootEshop.Models;

public class Cart
{
    public List<CartEntry> CartEntries { get; set; } = new();
}