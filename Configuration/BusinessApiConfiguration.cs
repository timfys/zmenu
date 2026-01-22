using System.ServiceModel;
using Menu4Tech.IBusinessAPIservice;

namespace Menu4Tech.Configuration
{
    public class BusinessApiConfiguration : MyConfiguration
    {
        public string EndpointAddress { get; set; }
        public int ol_EntityId { get; set; }
        public string ol_UserName { get; set; }
        public string ol_Password { get; set; }
        public int ol_AdminEntityId { get; set; }
        public string ol_AdminUserName { get; set; }
        public string ol_AdminPassword { get; set; }
        public int Timeout { get; set; }
        public int AdminBusinessId { get; set; }
        public int BusinessId { get; set; }
        public int MaxBufferSize { get; set; }

        public BusinessAPIClient InitClient()
        {
            var timeSpan = TimeSpan.FromSeconds(Timeout);
            var binding = new BasicHttpBinding
            {
                ReceiveTimeout = timeSpan,
                CloseTimeout = timeSpan,
                OpenTimeout = timeSpan,
                SendTimeout = timeSpan,
                MaxBufferSize = MaxBufferSize * 1024,
                MaxReceivedMessageSize = MaxBufferSize * 1024
            };
            var endpoint = new EndpointAddress(EndpointAddress);
            return new BusinessAPIClient(binding, endpoint);
        }
    }
    public class BusinessApi2Configuration : MyConfiguration
    {
        public string EndpointAddress { get; set; }
        public int ol_EntityId { get; set; }
        public string ol_UserName { get; set; }
        public string ol_Password { get; set; }
        public int ol_AdminEntityId { get; set; }
        public string ol_AdminUserName { get; set; }
        public string ol_AdminPassword { get; set; }
        public int Timeout { get; set; }
        public int AdminBusinessId { get; set; }
        public int BusinessId { get; set; }
        public int MaxBufferSize { get; set; }

        public BusinessAPIClient InitClient()
        {
            var timeSpan = TimeSpan.FromSeconds(Timeout);
            var binding = new BasicHttpBinding
            {
                ReceiveTimeout = timeSpan,
                CloseTimeout = timeSpan,
                OpenTimeout = timeSpan,
                SendTimeout = timeSpan,
                MaxBufferSize = MaxBufferSize * 1024,
                MaxReceivedMessageSize = MaxBufferSize * 1024
            };
            var endpoint = new EndpointAddress(EndpointAddress);
            return new BusinessAPIClient(binding, endpoint);
        }
    }
}