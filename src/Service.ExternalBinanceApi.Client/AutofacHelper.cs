using Autofac;
using Service.ExternalBinanceApi.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.ExternalBinanceApi.Client
{
    public static class AutofacHelper
    {
        public static void RegisterExternalBinanceApiClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new ExternalBinanceApiClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IHelloService>().SingleInstance();
        }
    }
}
