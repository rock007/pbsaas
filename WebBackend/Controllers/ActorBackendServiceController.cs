using ActorBackendService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Query;
using System;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebBackend.Config;

namespace WebBackend.Controllers
{
    
    [Route("api/[controller]")]
    public class ActorBackendServiceController : Controller
    {
        private readonly FabricClient fabricClient;
        private readonly ConfigSettings configSettings;
        private readonly StatelessServiceContext serviceContext;

        public ActorBackendServiceController(StatelessServiceContext serviceContext, ConfigSettings settings, FabricClient fabricClient)
        {
            this.serviceContext = serviceContext;
            this.configSettings = settings;
            this.fabricClient = fabricClient;
        }

        // GET: api/actorbackendservice
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            string serviceUri = this.serviceContext.CodePackageActivationContext.ApplicationName + "/" + this.configSettings.ActorBackendServiceName;

            ServicePartitionList partitions = await this.fabricClient.QueryManager.GetPartitionListAsync(new Uri(serviceUri));

            long count = 0;
            foreach (Partition partition in partitions)
            {
                long partitionKey = ((Int64RangePartitionInformation)partition.PartitionInformation).LowKey;
                IActorService actorServiceProxy = ActorServiceProxy.Create(new Uri(serviceUri), partitionKey);

                ContinuationToken continuationToken = null;

                do
                {
                    PagedResult<ActorInformation> page = await actorServiceProxy.GetActorsAsync(continuationToken, CancellationToken.None);

                    count += page.Items.Where(x => x.IsActive).LongCount();

                    continuationToken = page.ContinuationToken;
                }
                while (continuationToken != null);
            }

            return this.Json(new { Count = count ,Date=DateTime.Now} );
        }

        // POST api/actorbackendservice
        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
           
            string serviceUri = this.serviceContext.CodePackageActivationContext.ApplicationName + "/" + this.configSettings.ActorBackendServiceName;

            IActorBackendService proxy = ActorProxy.Create<IActorBackendService>(ActorId.CreateRandom(), new Uri(serviceUri));

            //await proxy.StartProcessingAsync(CancellationToken.None);

            await proxy.GetCountAsync(CancellationToken.None);

            return this.Json(true);
          
        }
    }
}