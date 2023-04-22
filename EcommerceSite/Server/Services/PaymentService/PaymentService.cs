using Stripe;
using Stripe.Checkout;

namespace EcommerceSite.Server.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;
        private readonly IOrderService _orderService;

        const string secret = "whsec_433bfd6f4170f0cc1096a17aec9130a1c851975668ecb0874cb4d2c605ca15f9";

        public PaymentService(ICartService cartService, IAuthService authService, IOrderService orderService) 
        {
            StripeConfiguration.ApiKey = "sk_test_51MuchyG9j9auPVBUQIH4d8Gu2fj5Roy8v0fBvtrjW1TIVLW8Nwqwsz7ULMOma9uwpiLGrHXepiVtAK7yyFhuPnaV00fUQFmWaY";

            _cartService = cartService;
            _authService = authService;
            _orderService = orderService;
        }
        public async Task<Session> CreateCheckoutSession()
        {
            var products = (await _cartService.GetCartProductsFromDB()).Data;
            var lineItems = new List<SessionLineItemOptions>();
            products.ForEach(product => lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = product.Price * 100,
                    Currency = "bgn",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.ProductTitle,
                        Images = new List<string> { product.ImageUrl }

                    }
                },
                Quantity = product.Quantity
            }));
            var options = new SessionCreateOptions
            {
                CustomerEmail = _authService.GetUserEmail(),
                ShippingAddressCollection = 
                    new SessionShippingAddressCollectionOptions
                    {
                        AllowedCountries = new List<string> {"BG"}
                    },
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:7252/order-success",
                CancelUrl = "https://localhost:7252/cart"
            };
            var service = new SessionService();
            Session session = service.Create(options);
            return session;
        }

        public async Task<ServiceResponse<bool>> FulfillOrder(HttpRequest request)
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync();
            try 
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    request.Headers["Stripe-Signature"],
                    secret
                    );
                if(stripeEvent.Type == Events.CheckoutSessionCompleted) 
                { 
                    var session = stripeEvent.Data.Object as Session;
                    var user = await _authService.GetUserByEmail(session.CustomerEmail);
                    await _orderService.OrderPlace(user.Id);
                }
                return new ServiceResponse<bool> { Data = true };
            }
            catch(StripeException e)
            {
                return new ServiceResponse<bool> { Data = false, Success = false, Message = e.Message };
            }
        }
    }
}
