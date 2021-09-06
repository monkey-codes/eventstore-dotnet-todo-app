using System;
using Autofac;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Query.Projection
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
                await _client.CreateContinuousAsync(_name, _js);
            }
            catch (InvalidOperationException e) when (e.Message.Contains("Conflict"))
            {
                var format = $"{_name} already exists, updating...";
                _logger.LogInformation(format);
                await _client.UpdateAsync(_name, _js);
                await _client.ResetAsync(_name);
            }
        }
    }
}