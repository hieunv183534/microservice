using AutoMapper;
using EventBus.MessageComponents.Consumers.Basket;
using MassTransit;
using MediatR;
using Ordering.Application.Features.V1.Orders;
using ILogger = Serilog.ILogger;

namespace Ordering.API.EventBusConsumer;

public class BasketCheckoutConsumer : IBasketCheckoutConsumer
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public BasketCheckoutConsumer(IMediator mediator, IMapper mapper, ILogger logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        var command = _mapper.Map<CreateOrderCommand>(context.Message);
        var result = await _mediator.Send(command);
        _logger.Information("BasketCheckoutEvent consumed successfully. " +
                            "Order is created with Id: {newOrderId}", result.Data);
    }
}