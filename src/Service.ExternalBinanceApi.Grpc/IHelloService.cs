using System.ServiceModel;
using System.Threading.Tasks;
using Service.ExternalBinanceApi.Grpc.Models;

namespace Service.ExternalBinanceApi.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}