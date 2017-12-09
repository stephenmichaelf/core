﻿using System.Threading.Tasks;
using Bit.Core.Repositories;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Bit.Core.Models.Data;

namespace Bit.Core.Services
{
    public class AzureQueueEventWriteService : IEventWriteService
    {
        private readonly CloudQueue _queue;
        private readonly GlobalSettings _globalSettings;

        private JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public AzureQueueEventWriteService(
            IEventRepository eventRepository,
            GlobalSettings globalSettings)
        {
            var storageAccount = CloudStorageAccount.Parse(globalSettings.Storage.ConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();

            _queue = queueClient.GetQueueReference("event");
            _globalSettings = globalSettings;
        }

        public async Task CreateAsync(IEvent e)
        {
            var json = JsonConvert.SerializeObject(e, _jsonSettings);
            var message = new CloudQueueMessage(json);
            await _queue.AddMessageAsync(message);
        }

        public async Task CreateManyAsync(IList<IEvent> e)
        {
            var json = JsonConvert.SerializeObject(e, _jsonSettings);
            var message = new CloudQueueMessage(json);
            await _queue.AddMessageAsync(message);
        }
    }
}
