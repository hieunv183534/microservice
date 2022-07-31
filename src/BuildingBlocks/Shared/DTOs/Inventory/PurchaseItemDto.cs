using Shared.Enums.Inventory;

namespace Shared.DTOs.Inventory;

public record PurchaseItemDto
{
    public EDocumentType DocumentType => EDocumentType.Purchase;
    
    public string ItemNo { get; set; }
    
    public int Quantity { get; set; }
}