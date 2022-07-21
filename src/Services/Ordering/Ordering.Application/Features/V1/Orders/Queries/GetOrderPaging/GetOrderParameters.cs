using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrderParameters : PagingRequestParameters
{
    public GetOrderParameters()
    {
        OrderBy = "CreatedDate desc";
    }
    
    public string? SearchTerm { get; set; }
}