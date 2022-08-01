namespace Shared.DTOs.Inventory;

public record GetInventoryPagingQuery
{
    public string? SearchKeyword { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}