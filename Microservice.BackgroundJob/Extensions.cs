using System.ComponentModel;
using System.Threading.Tasks;
using Hangfire;
using MediatR;
using Newtonsoft.Json;

namespace Microservice.BackgroundJob
{
    public static class Extensions
    {
        public class MediatorHangfireBridge
        {
            private readonly IMediator _mediator;

            public MediatorHangfireBridge(IMediator mediator)
            {
                _mediator = mediator;
            }

            public async Task Send(IRequest command)
            {
                await _mediator.Send(command);
            }

            [DisplayName("{0}")]
            public async Task Send(string jobName, IRequest command)
            {
                await _mediator.Send(command);
            }
        }
        
        public static void Enqueue(this IMediator mediator, string jobName, IRequest request)
        {
            var client = new BackgroundJobClient();
            client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(jobName, request));
        }

        public static void Enqueue(this IMediator mediator, IRequest request)
        {
            var client = new BackgroundJobClient();
            client.Enqueue<MediatorHangfireBridge>(bridge => bridge.Send(request));
        }
        
        public static void Recurring(this IMediator mediator, IRequest request, string cron)
        {
            RecurringJob.AddOrUpdate<MediatorHangfireBridge>(bridge => bridge.Send(request), cron);
        }
        
        public static void Recurring(this IMediator mediator, IRequest request, string cron, string jobName)
        {
            RecurringJob.AddOrUpdate<MediatorHangfireBridge>(bridge => bridge.Send(jobName, request), cron);
        }
        
        public static IGlobalConfiguration UseMediatR(this IGlobalConfiguration configuration)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            configuration.UseSerializerSettings(jsonSettings);

            return configuration;
        }
    }
}
