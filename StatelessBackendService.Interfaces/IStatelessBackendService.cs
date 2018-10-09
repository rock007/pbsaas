using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;

namespace StatelessBackendService.Interfaces
{
    public interface IStatelessBackendService : IService
    {
        Task<long> GetCountAsync();
    }
}
