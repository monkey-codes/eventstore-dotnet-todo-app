using System;
using System.Threading;
using Autofac;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Query.UsingProjection
{
    public abstract class AbstractProjectionManager : IStartable
    {
        private readonly EventStoreProjectionManagementClient _client;
        private readonly ILogger<AbstractProjectionManager> _logger;
        private readonly string _name;
        private readonly string _js;

        protected AbstractProjectionManager(EventStoreProjectionManagementClient client,
            ILogger<AbstractProjectionManager> logger, string name, string js)
        {
            _client = client;
            _logger = logger;
            _name = name;
            _js = js;
        }

        public async void Start()
        {
            
            try
            {
                //Hack to get around the race condition of CreateContinuousAsync simultaneously from 2 separate threads
                //TODO One solution may be to just have global ProjectionManager that sequentially registers all the projections.
                Thread.Sleep(new Random().Next(100, 2000));
                await _client.CreateContinuousAsync(_name, _js);
            }
            catch (InvalidOperationException e) when (e.Message.Contains("Conflict"))
            {
                var format = $"{_name} already exists, updating...";
                _logger.LogInformation(format);
                await _client.UpdateAsync(_name, _js);
                await _client.ResetAsync(_name);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Could not create {name} projection: {message}", _name, e.Message);
            }
        }
    }
}