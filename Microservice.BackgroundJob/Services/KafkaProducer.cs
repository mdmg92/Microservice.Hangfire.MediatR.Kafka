using System.Threading.Tasks;
using DotNetCore.CAP;

namespace Microservice.BackgroundJob.Services
{
    public class KafkaProducer
    {
        private readonly ICapPublisher _kafka;

        public KafkaProducer(ICapPublisher kafka)
        {
            _kafka = kafka;
        }

        public async Task Send<T>(T message)
        {
            await _kafka.PublishAsync(typeof(T).Name, message);
        }
        
        public async Task Send<T>(T message, string topic)
        {
            await _kafka.PublishAsync(topic, message);
        }
    }
}
