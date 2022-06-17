using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IResult> GetCustomerByUsernameAsync(string username) => 
        Results.Ok(await _repository.GetCustomerByUsernameAsync(username));
}