using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microservice.BackgroundJob.Messages;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Microservice.BackgroundJob.Services
{
    public class MongoService
    {
        private readonly IMongoCollection<ActualizarSaldoCollection> _mongoCollection;
        public MongoService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("AppConnection"));

            var db = client.GetDatabase("app");

            _mongoCollection = db.GetCollection<ActualizarSaldoCollection>(nameof(ActualizarSaldo));
        }

        public async Task<ActualizarSaldo> Save(ActualizarSaldo dto, Expression<Func<ActualizarSaldoCollection, bool>> exp = null)
        {
            var document = new ActualizarSaldoCollection
            {
                Cuenta = dto.Cuenta,
                Id = dto.Id,
                Saldo = dto.Saldo,
                Procesado = dto.Procesado
            };

            if (await ExistBy(exp is null ? c => c.Id == document.Id : exp))
            {
                await _mongoCollection.ReplaceOneAsync(exp is null ? c => c.Id == document.Id : exp, document);
            }
            else
            {
                await _mongoCollection.InsertOneAsync(document);
            }

            return dto;
        }

        public async Task<bool> ExistBy(Expression<Func<ActualizarSaldoCollection, bool>> exp = null)
        {
            return await _mongoCollection.Find(exp).AnyAsync();
        }

        public async Task<IEnumerable<ActualizarSaldoCollection>> GetPendientes()
        {
            return (await _mongoCollection.FindAsync(_ => _.Procesado == false)).ToEnumerable();
        }
    }

    public class ActualizarSaldoCollection : ActualizarSaldo
    {
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
