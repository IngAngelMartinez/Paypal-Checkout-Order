using API.Configurations;
using API.Interfaces;
using API.Requests;
using API.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.Services
{
    public class PaypalAPIService : IPaypalService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly PaypalConfiguration paypalConfiguration;
        private readonly ILogger<PaypalAPIService> logger;

        public PaypalAPIService(IHttpClientFactory httpClientFactory, IOptionsMonitor<PaypalConfiguration> paypalConfiguration, ILogger<PaypalAPIService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.paypalConfiguration = paypalConfiguration.CurrentValue;
            this.logger = logger;
        }

        public async Task<OrderResponse> CreateOrder(CreateOrderRequest createOrderRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderResponse> GetOrder(string orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderResponse> CaptureOrder(string orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderResponse> AuthorizeOrder(string orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetRefreshToken(string code)
        {
            throw new NotImplementedException();
        }

        public async Task<TokenResponse> GetToken()
        {
            throw new NotImplementedException();
        }

    }
}
