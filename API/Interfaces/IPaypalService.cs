using API.Requests;
using API.Responses;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IPaypalService
    {
        Task<OrderResponse> CreateOrder(CreateOrderRequest createOrderRequest);
        Task<OrderResponse> GetOrder(string orderId);
        Task<OrderResponse> CaptureOrder(string orderId);
        Task<OrderResponse> AuthorizeOrder(string orderId);
        Task<TokenResponse> GetToken();
        Task<object> GetRefreshToken(string code);
    }
}
