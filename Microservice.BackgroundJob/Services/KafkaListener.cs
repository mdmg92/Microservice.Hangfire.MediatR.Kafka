using System.Threading.Tasks;
using DotNetCore.CAP;
using Hangfire;
using MediatR;
using Microservice.BackgroundJob.Messages;

namespace Microservice.BackgroundJob.Services
{
    public class KafkaListener : ICapSubscribe
    {
        private readonly MongoService _mongo;
        private readonly IMediator _mediator;

        public KafkaListener(MongoService mongo, IMediator mediator)
        {
            _mongo = mongo;
            _mediator = mediator;
        }

        [CapSubscribe(nameof(ActualizarSaldo))]
        // [CapSubscribe("ActualizarSaldo")]
        public async Task Handle(ActualizarSaldo request)
        {
            await _mongo.Save(request);
            
            _mediator.Recurring(new IntegrationService.Request(), Cron.Minutely());
        }
    }
}
