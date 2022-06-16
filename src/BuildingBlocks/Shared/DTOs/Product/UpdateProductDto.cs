namespace Shared.DTOs.Product;

public class UpdateProductDto
{
    public string Name { get; set; }
    
    public string Summary { get; set; }
    
    public string Description { get; set; }
    
    public decimal Price { get; set; }
}