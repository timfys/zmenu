using Menu4Tech.Helper;
using Menu4Tech.IBusinessAPIservice;
using Menu4Tech.Models.Interfaces;
using Microsoft.AspNetCore.Http.Extensions;
using Wangkanai.Detection.Services;

namespace Menu4Tech.Services
{
    public class LogRequestService : ILogRequest
    {
        private readonly IDetectionService _detectionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICredential _credential;
        public LogRequestService(IDetectionService detectionService, IHttpContextAccessor httpContextAccessor, ICredential credential)
        {
            _detectionService = detectionService;
            _httpContextAccessor = httpContextAccessor;
            _credential = credential;
        }

        public async Task EntityLogAsync(int entityId,string ipaddress)
        {
            var fullUrl = _httpContextAccessor.HttpContext.Request.GetDisplayUrl();
            var userIp = _httpContextAccessor.HttpContext.Request.Headers["CF-Connecting-IP"];// _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"];
            var deviceId = (int)_detectionService.Device.Type;
            var config = EnvironmentHelper.BusinessApiConfiguration;
            var client = config.InitClient();
            await client.Entity_LogtrafficAsync(new Entity_LogtrafficRequest(entityId, deviceId, userAgent, userIp, fullUrl, Array.Empty<string>(), Array.Empty<string>()));
        }
    }
}
