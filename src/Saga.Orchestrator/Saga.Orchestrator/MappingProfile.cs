using AutoMapper;
using EventBus.Messages.IntegrationEvents.Events;
using Shared.DTOs.Basket;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;

namespace Saga.Orchestrator;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<BasketCheckoutDto, CreateOrderDto>();
        CreateMap<CartItemDto, SaleItemDto>();
        CreateMap<BasketCheckoutEvent, BasketCheckoutDto>();
    }
}