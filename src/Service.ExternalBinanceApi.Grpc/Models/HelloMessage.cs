using System.Runtime.Serialization;
using Service.ExternalBinanceApi.Domain.Models;

namespace Service.ExternalBinanceApi.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}