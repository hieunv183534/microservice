using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Basket;

namespace Saga.Orchestrator.HttpRepository;

public class BasketHttpRepository : IBasketHttpRepository
{
    private readonly HttpClient _client;
    
    public BasketHttpRepository(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<CartDto> GetBasket(string username)
    {
        var cart = await _client.GetFromJsonAsync<CartDto>(username);
        if (cart == null || !cart.Items.Any()) return null;

        return cart;
    }

    public Task<bool> DeleteBasket(string username)
    {
        throw new NotImplementedException();
    }
}