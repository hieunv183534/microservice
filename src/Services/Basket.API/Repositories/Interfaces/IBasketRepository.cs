using Basket.API.Entities;

namespace Basket.API.Repositories.Interfaces;

public interface IBasketRepository
{
    Task<Cart> GetBasketByUserName(string userName);
    Task<Cart> UpdateBasket(Cart basket);
    Task DeleteBasketFromUserName(string userName);
}