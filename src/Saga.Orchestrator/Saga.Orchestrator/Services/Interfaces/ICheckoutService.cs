using Shared.DTOs.Basket;

namespace Saga.Orchestrator.Services.Interfaces;

public interface ICheckoutSagaService
{
    Task<bool> Checkout(string username, BasketCheckoutDto basketCheckout);
}