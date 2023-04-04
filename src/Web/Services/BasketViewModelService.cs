using System.Security.Claims;
using Web.Interfaces;

namespace Web.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IBasketService _basketService;
        private readonly IHttpContextAccessor _httpContextAccessor;

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
            IHttpContextAccessor httpContextAccessor)
        {
            _basketService = basketService;
            _httpContextAccessor = httpContextAccessor;
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


    }
}
