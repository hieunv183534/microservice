using AutoMapper;
using Contracts.Services;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Serilog;
using Shared.SeedWork;
using Shared.Services.Email;

namespace Ordering.Application.Features.V1.Orders;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResult<long>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly ISmtpEmailService _emailService;
    private readonly ILogger _logger;

    public CreateOrderCommandHandler(IOrderRepository orderRepository,
        IMapper mapper,
        ISmtpEmailService emailService,
        ILogger logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private const string MethodName = "CreateOrderCommandHandler";

    public async Task<ApiResult<long>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.Information($"BEGIN: {MethodName} - Username: {request.UserName}");
        var orderEntity = _mapper.Map<Order>(request);
        var addedOrder = await _orderRepository.CreateOrderAsync(orderEntity);
        addedOrder.AddedOrder();
        await _orderRepository.SaveChangesAsync();
        _logger.Information($"Order {addedOrder.Id} is successfully created.");

        // SendEmail(addedOrder, cancellationToken);

        _logger.Information($"END: {MethodName} - Username: {request.UserName}");
        return new ApiSuccessResult<long>(addedOrder.Id);
    }

    private void SendEmail(Order order, CancellationToken cancellationToken)
    {
        var emailRequest = new MailRequest
        {
            ToAddress = order.EmailAddress,
            Body = $"Your order detail. " +
                   $"<p> Order Id: {order.Id}</p>" +
                   $"<p> Total: {order.TotalPrice}</p>",
            Subject = $"Hello {order.FullName}, your order was created"
        };

        try
        { 
            _emailService.SendEmailAsync(emailRequest, cancellationToken);
            _logger.Information($"Sent Created Order to email {order.EmailAddress}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Order {order.Id} failed due to an error with the email service: {ex.Message}");
        }
    }
}