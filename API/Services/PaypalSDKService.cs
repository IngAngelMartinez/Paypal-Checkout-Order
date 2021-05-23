using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using API.Requests;
using API.Responses;
using API.Configurations;
using Microsoft.Extensions.Options;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using Microsoft.Extensions.Logging;

namespace API.Services
{
    public class PaypalSDKService : IPaypalService
    {
        private readonly PaypalConfiguration paypalConfiguration;
        private readonly ILogger<PaypalSDKService> logger;

        public PaypalSDKService(IOptionsMonitor<PaypalConfiguration> paypalConfiguration, ILogger<PaypalSDKService> logger)
        {
            this.paypalConfiguration = paypalConfiguration.CurrentValue;
            this.logger = logger;
        }


        public async Task<OrderResponse> CreateOrder(CreateOrderRequest createOrderRequest)
        {

            OrderResponse orderResponse = new OrderResponse();

            try
            {

                var newOrder = CreateNewOrder(createOrderRequest);

                var orderRequest = new OrdersCreateRequest();
                orderRequest.Prefer("return=representation");
                orderRequest.RequestBody(newOrder);

                HttpResponse orderHttpResponse = await GetClient().Execute(orderRequest);

                var orderStatusCode = orderHttpResponse.StatusCode;
                Order order = orderHttpResponse.Result<Order>();

                orderResponse = CreateOrderResponse(order);
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Cant't create order");
            }

            return orderResponse;

        }

        public async Task<OrderResponse> GetOrder(string orderId)
        {
            OrderResponse orderResponse = new OrderResponse();

            try
            {
                OrdersGetRequest ordersGetRequest = new OrdersGetRequest(orderId);

                var client = GetClient();

                HttpResponse ordersGetResponse = await client.Execute(ordersGetRequest);

                var ordersGetStatusCode = ordersGetResponse.StatusCode;
                Order order = ordersGetResponse.Result<Order>();

                orderResponse.IdOrder = order.Id;
                orderResponse.LinkResponses = order.Links.Select(T => new LinkResponse { Url = T.Href, Method = T.Method, Rel = T.Rel }).ToList();
                orderResponse.Status = order.Status;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Cant't get order {orderId}");
            }

            return orderResponse;

        }

        public async Task<OrderResponse> CaptureOrder(string orderId)
        {
            OrderResponse orderResponse = new OrderResponse();

            try
            {
                var request = new OrdersCaptureRequest(orderId);
                request.RequestBody(new OrderActionRequest());

                HttpResponse response = await GetClient().Execute(request);

                var statusCode = response.StatusCode;
                Order order = response.Result<Order>();

                orderResponse = CreateOrderResponse(order);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Cant't capture order {orderId}");
            }
            
            return orderResponse;
        }

        public async Task<OrderResponse> AuthorizeOrder(string orderId)
        {
            OrderResponse orderResponse = new OrderResponse();

            try
            {
                
                var request = new OrdersAuthorizeRequest(orderId);
                request.Prefer("return=representation");
                request.RequestBody(new AuthorizeRequest());

                HttpResponse response = await GetClient().Execute(request);

                var statusCode = response.StatusCode;
                Order order = response.Result<Order>();

                orderResponse = CreateOrderResponse(order);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Cant't authorize order {orderId}");
            }

            return orderResponse;
        }

        public async Task<TokenResponse> GetToken()
        {
            TokenResponse tokenResponse = new TokenResponse();

            try
            {
                PayPalEnvironment environment = GetEnvironment();

                PayPalHttpClient client = new PayPalHttpClient(environment);

                var tokenRequest = new AccessTokenRequest(environment);

                HttpResponse getTokenResponse = await client.Execute(tokenRequest);

                var tokenStatusCode = getTokenResponse.StatusCode;
                AccessToken accessToken = getTokenResponse.Result<AccessToken>();

                tokenResponse.ExpiresIn = accessToken.ExpiresIn;
                tokenResponse.Token = accessToken.Token;
                tokenResponse.TokenType = accessToken.TokenType;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Cant't get token");
            }

            return tokenResponse;
        }

        public async Task<object> GetRefreshToken(string code)
        {
            RefreshToken refreshToken = new RefreshToken();

            try
            {
                var environment = GetEnvironment();
                var refreshTokenRequest = new RefreshTokenRequest(environment, code);

                var client = GetClient();

                var refreshTokenResponse = await client.Execute(refreshTokenRequest);

                var refreshTokenStatusCode = refreshTokenResponse.StatusCode;

                refreshToken = refreshTokenResponse.Result<RefreshToken>();


            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Cant't get refresh token");
            }

            return refreshToken;

        }


        private OrderResponse CreateOrderResponse(Order order)
        {
            OrderResponse orderResponse = new OrderResponse();

            orderResponse.IdOrder = order.Id;
            orderResponse.LinkResponses = order.Links.Select(T => new LinkResponse { Url = T.Href, Method = T.Method, Rel = T.Rel }).ToList();
            orderResponse.Status = order.Status;

            return orderResponse;

        }

        private OrderRequest CreateNewOrder(CreateOrderRequest createOrderRequest)
        {
            OrderRequest orderRequest = new OrderRequest();

            try
            {

                #region Payee

                PayPalCheckoutSdk.Orders.Payee payee = new PayPalCheckoutSdk.Orders.Payee()
                {
                    PayeeDisplayable = new PayeeDisplayable()
                    {
                        BrandName = paypalConfiguration.Payee.BrandName,
                        BusinessPhone = new Phone()
                        {
                            CountryCallingCode = paypalConfiguration.Payee.CountryCallingCode,
                            ExtensionNumber = paypalConfiguration.Payee.ExtensionNumber,
                            NationalNumber = paypalConfiguration.Payee.PhoneNumber,
                        },
                        Email = paypalConfiguration.Payee.Email,
                    },
                };

                #endregion

                #region Items

                List<PayPalCheckoutSdk.Orders.Item> items = new List<PayPalCheckoutSdk.Orders.Item>();

                decimal totalValue = 0;

                foreach (var itemRequest in createOrderRequest.Items)
                {
                    #region Tax

                    PayPalCheckoutSdk.Orders.Money tax = new PayPalCheckoutSdk.Orders.Money();

                    tax.CurrencyCode = itemRequest.Tax.CurrencyCode;
                    tax.Value = itemRequest.Tax.Value;

                    #endregion

                    #region UnitAmount

                    PayPalCheckoutSdk.Orders.Money unitAmount = new PayPalCheckoutSdk.Orders.Money();

                    unitAmount.CurrencyCode = itemRequest.UnitAmount.CurrencyCode;
                    unitAmount.Value = itemRequest.UnitAmount.Value;

                    #endregion

                    #region item

                    PayPalCheckoutSdk.Orders.Item item = new PayPalCheckoutSdk.Orders.Item();

                    item.Category = itemRequest.Category;
                    item.Description = itemRequest.Description;
                    item.Name = itemRequest.Name;
                    item.Quantity = itemRequest.Quantity;
                    item.Sku = itemRequest.Sku;
                    item.Tax = tax;
                    item.UnitAmount = unitAmount;

                    #endregion

                    totalValue += decimal.Parse(item.UnitAmount.Value) * int.Parse(item.Quantity);

                    items.Add(item);

                }

                #endregion

                #region AmountBreakdown 

                AmountBreakdown amountBreakdown = new AmountBreakdown() {
                    ItemTotal = new PayPalCheckoutSdk.Orders.Money()
                    {
                        CurrencyCode = createOrderRequest.CurrencyCode,
                        Value = totalValue.ToString()
                    },
                    
                };

                #endregion

                #region PurchaseUnitRequest

                PurchaseUnitRequest purchaseUnitRequest = new PurchaseUnitRequest()
                {
                    AmountWithBreakdown = new AmountWithBreakdown()
                    {
                        Value = totalValue.ToString(),
                        CurrencyCode = createOrderRequest.CurrencyCode,
                        AmountBreakdown = amountBreakdown,
                        
                    },
                    Description = "Thanks to buy in my shop",
                    Payee = payee,
                    Items = items,
                    
                };

                #endregion


                orderRequest = new OrderRequest()
                {
                    CheckoutPaymentIntent = "CAPTURE",
                    PurchaseUnits = new List<PurchaseUnitRequest>()
                    {
                        purchaseUnitRequest
                    },
                    ApplicationContext = new ApplicationContext()
                    {
                        ReturnUrl = paypalConfiguration.WebSite.ReturnUrl,
                        CancelUrl = paypalConfiguration.WebSite.CancelUrl,
                        BrandName = paypalConfiguration.Payee.BrandName,
                        UserAction = "PAY_NOW",
                        ShippingPreference = "NO_SHIPPING",
                        Locale = "es-MX",
                        LandingPage = "NO_PREFERENCE",
                    },
                };


            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Cant't asign request to order");
            }

            return orderRequest;

        }

        private PayPalEnvironment GetEnvironment()
        {
            PayPalEnvironment environment;

            if (paypalConfiguration.SandboxEnvironment)
            {
                environment = new SandboxEnvironment(paypalConfiguration.Credentials.ClientID, paypalConfiguration.Credentials.Secret);
            }
            else
            {
                environment = new PayPalEnvironment(paypalConfiguration.Credentials.ClientID, paypalConfiguration.Credentials.Secret, paypalConfiguration.WebSite.BaseUrl, paypalConfiguration.WebSite.WebUrl);
            }

            return environment;
        }

        private PayPalHttpClient GetClient()
        {
            PayPalEnvironment environment = GetEnvironment();

            PayPalHttpClient client = new PayPalHttpClient(environment);

            return client;
        }

    }
}
