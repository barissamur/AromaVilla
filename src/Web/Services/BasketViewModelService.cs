using System.Security.Claims;
using Web.Interfaces;

namespace Web.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IBasketService _basketService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderService _orderService;

        private HttpContext? HttpContext => _httpContextAccessor.HttpContext;
        private string? UserId => HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier); // giriş yapanın id'si
        private string? AnonId => HttpContext?.Request.Cookies[Constants.BASKET_COOKIENAME]; //cookie'nin adı
        private string BuyerId => UserId ?? AnonId ?? CreateAnonymousId();

        private string _createAnonId = null!;
        private string CreateAnonymousId()
        {
            if (_createAnonId == null)
            {
                _createAnonId = Guid.NewGuid().ToString();
                HttpContext?.Response.Cookies.Append(Constants.BASKET_COOKIENAME, _createAnonId, new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(14), // 14 gün geçerli olsun
                    IsEssential = true // gerekli bir cookie mi?
                });
            }

            return _createAnonId;
        }

        public BasketViewModelService(IBasketService basketService,
            IHttpContextAccessor httpContextAccessor,
            IOrderService orderService)
        {
            _basketService = basketService;
            _httpContextAccessor = httpContextAccessor;
            _orderService = orderService;
        }
        public async Task<BasketViewModel> AddItemToBasketAsync(int productId, int quantity)
        {
            var basket = await _basketService.AddItemToBasketAsync(BuyerId, productId, quantity);
            return basket.ToBasketViewModel();

        }

        public async Task<BasketViewModel> GetBasketViewModelAsync()
        {
            var basket = await _basketService.GetOrCreateBasketAsync(BuyerId);

            return basket.ToBasketViewModel();
        }

        public async Task EmptyBasketAsync()
        {
            await _basketService.EmptyBasketAsync(BuyerId);
        }

        public async Task DeleteBasketItemAsync(int productId)
        {
            await _basketService.DeleteBasketItemAsync(BuyerId, productId);
        }

        public async Task UpdateBasketAsync(Dictionary<int, int> quantities)
        {
            await _basketService.SetQuantities(BuyerId, quantities);
        }

        public async Task TransferBasketAsyc()
        {
            if (AnonId == null || UserId == null) return;
            await _basketService.TransferBasketAsync(AnonId, UserId);
            HttpContext?.Response.Cookies.Delete(Constants.BASKET_COOKIENAME); // cookie'yi siliyoruz
        }

        public async Task CheckoutAsync(string street, string city, string? state, string country, string zipCode)
        {
            var shippingAddress = new Address(street, city, state, country, zipCode);
            await _orderService.CreateOrderAsync(BuyerId, shippingAddress);
            await _basketService.EmptyBasketAsync(BuyerId);
        }
    }
}
