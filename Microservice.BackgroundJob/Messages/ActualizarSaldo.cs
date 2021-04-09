using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microservice.BackgroundJob.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microservice.BackgroundJob.Messages
{
    public class ActualizarSaldo : IRequest
    {
        public string Id { get; set; }
        public string Cuenta { get; set; }
        public double Saldo { get; set; }
        public bool Procesado { get; set; } = false;

        public class Handler : AsyncRequestHandler<ActualizarSaldo>
        {
            private readonly MongoService _mongo;
            private readonly ILogger<Handler> _logger;

            public Handler(MongoService mongo, ILogger<Handler> logger)
            {
                _mongo = mongo;
                _logger = logger;
            }
            
            protected override async Task Handle(ActualizarSaldo request, CancellationToken cancellationToken)
            {
                System.Threading.Thread.Sleep(2000);

                request.Procesado = true;
                
                _logger.LogInformation("--- Actualizacion de saldo procesada");
                
                await _mongo.Save(request, f => f.Id == request.Id);

                var json = JsonConvert.SerializeObject(request);
                
                _logger.LogInformation($"--- Comando procesado {json}");
            }
        }
    }
}