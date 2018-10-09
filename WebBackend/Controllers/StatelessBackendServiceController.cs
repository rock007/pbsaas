using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StatelessBackendService.Interfaces;
using System.Fabric;
using System;
using WebBackend.Config;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace WebBackend.Controllers
{
    
    [Route("api/[controller]")]
    public class StatelessBackendServiceController : Controller
    {
        private readonly ConfigSettings configSettings;
        private readonly StatelessServiceContext serviceContext;

        public StatelessBackendServiceController(StatelessServiceContext serviceContext, ConfigSettings settings)
        {
            this.serviceContext = serviceContext;
            this.configSettings = settings;
        }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            string serviceUri = this.serviceContext.CodePackageActivationContext.ApplicationName + "/" + this.configSettings.StatelessBackendServiceName;

            IStatelessBackendService proxy = ServiceProxy.Create<IStatelessBackendService>(new Uri(serviceUri));

            long result = await proxy.GetCountAsync();

            return this.Json(new  { Count = result ,Time = DateTime.Now});
        }
    }
}