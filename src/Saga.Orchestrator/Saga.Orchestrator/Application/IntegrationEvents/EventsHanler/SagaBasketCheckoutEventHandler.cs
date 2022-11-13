using AutoMapper;
using Contracts.Sagas.OrderManager;
using EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Saga.Orchestrator.OrderManager;
using Shared.DTOs.Basket;
using ILogger = Serilog.ILogger;

namespace Saga.Orchestrator.Application.IntegrationEvents.EventsHanler
{
    public class SagaBasketCheckoutEventHandler : IConsumer<BasketCheckoutEvent>
    {
        private readonly ISagaOrderManager<BasketCheckoutDto, OrderResponse> _sagaOrderManager;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public SagaBasketCheckoutEventHandler(ISagaOrderManager<BasketCheckoutDto, OrderResponse> sagaOrderManager, ILogger logger, IMapper mapper)
        {
            _logger = logger;
            _sagaOrderManager = sagaOrderManager;
            _mapper = mapper;
        }

        public Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            _logger.Information("BasketCheckoutEvent consumed successfully. " + context.Message);
            var basketCheckoutDto = _mapper.Map<BasketCheckoutDto>(context.Message);
            _sagaOrderManager.CreateOrder(basketCheckoutDto);
            return Task.CompletedTask;
        }
    }
}
