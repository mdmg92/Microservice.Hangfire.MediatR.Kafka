using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microservice.BackgroundJob.Messages;
using Microsoft.Extensions.Logging;

namespace Microservice.BackgroundJob.Services
{
    public static class IntegrationService
    {
        public class Request : IRequest {}

        public class Handler : AsyncRequestHandler<Request>
        {
            private readonly MongoService _mongo;
            private readonly IMediator _mediator;
            private readonly ILogger<ActualizarSaldo.Handler> _logger;

            public Handler(MongoService mongo, IMediator mediator, ILogger<ActualizarSaldo.Handler> logger)
            {
                _mongo = mongo;
                _mediator = mediator;
                _logger = logger;
            }

            protected override async Task Handle(Request request, CancellationToken cancellationToken)
            {
                _logger.LogInformation("--- Buscando pendientes");
                
                var pendientes = await _mongo.GetPendientes();

                var counter = 0;
                foreach (var pendiente in pendientes)
                {
                    await _mediator.Send(new ActualizarSaldo
                    {
                        Cuenta = pendiente.Cuenta,
                        Id = pendiente.Id,
                        Saldo = pendiente.Saldo
                    });

                    counter++;
                }
                
                _logger.LogInformation($"--- Procesados {counter} pendientes");
            }
        }
    }
}