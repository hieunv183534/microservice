using AutoMapper;
using EventBus.MessageComponents.Consumers.Basket;
using MassTransit;
using MediatR;
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
    
    public Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        _logger.Information("BasketCheckoutEvent consumed successfully. Id: {MessageId}", context.Message.Id);
        
        return Task.CompletedTask;
    }
}