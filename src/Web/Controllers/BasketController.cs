using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketViewModelService _basketViewModelService;

        public BasketController(IBasketViewModelService basketViewModelService)
        {
            _basketViewModelService = basketViewModelService;
        }
        public async Task<IActionResult> AddToBasket(int productId, int quantity = 1)
        {
            var vm = await _basketViewModelService.AddItemToBasketAsync(productId, quantity);
            return Json(vm);
        }

        public async Task<IActionResult> Index()
        {
            var vm = await _basketViewModelService.GetBasketViewModelAsync();
            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var basket = await _basketViewModelService.GetBasketViewModelAsync();
            var vm = new CheckoutViewModel()
            {
                Basket = basket
            };
            return View(vm);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // payment

                await _basketViewModelService.CheckoutAsync(vm.Street, vm.City, vm.State, vm.Country, vm.ZipCode);
                return RedirectToAction("OrderConfirmed");
            }

            vm.Basket = await _basketViewModelService.GetBasketViewModelAsync();
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Empty()
        {
            await _basketViewModelService.EmptyBasketAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int productId)
        {
            await _basketViewModelService.DeleteBasketItemAsync(productId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        // <input name =quantities[3]" value="12"> dictioanry içindeki intler [3] ve 12'den geliyo name ile bunu "quantities" olarak kısıtlıyoruz
        public async Task<IActionResult> Update([ModelBinder(Name = "quantities")] Dictionary<int, int> quantities)
        {
            await _basketViewModelService.UpdateBasketAsync(quantities);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> OrderConfirmed()
        {
            await _basketViewModelService.EmptyBasketAsync();
            return View();
        }
    }
}
