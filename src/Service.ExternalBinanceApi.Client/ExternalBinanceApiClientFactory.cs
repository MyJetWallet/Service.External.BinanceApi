using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.ExternalBinanceApi.Grpc;

namespace Service.ExternalBinanceApi.Client
{
    [UsedImplicitly]
    public class ExternalBinanceApiClientFactory: MyGrpcClientFactory
    {
        public ExternalBinanceApiClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IHelloService GetHelloService() => CreateGrpcService<IHelloService>();
    }
}
