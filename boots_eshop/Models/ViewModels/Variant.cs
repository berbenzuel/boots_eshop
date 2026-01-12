using Microsoft.AspNetCore.Mvc.Rendering;

namespace BootEshop.Models;

public class Variant
{
    public Guid stockId { get; set; }
    public SelectListItem Color {get; set;}
    public SelectListItem Size {get; set;}
}