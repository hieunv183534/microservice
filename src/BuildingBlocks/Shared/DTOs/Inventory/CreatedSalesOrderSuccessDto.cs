namespace Shared.DTOs.Inventory;

public class CreatedSalesOrderSuccessDto
{
    public CreatedSalesOrderSuccessDto(string documentNo)
    {
        DocumentNo = documentNo;
    }
    public string DocumentNo { get; private set; }
}