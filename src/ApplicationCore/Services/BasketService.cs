using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class BasketService : IBasketService
    {
        private readonly IRepository<Basket> _basketRepo;
        private readonly IRepository<BasketItem> _basketItemRepo;
        private readonly IRepository<Product> _productRepo;

        public BasketService(IRepository<Basket> basketRepo,
            IRepository<BasketItem> basketItemRepo,
            IRepository<Product> productRepo)
        {
            _basketRepo = basketRepo;
            _basketItemRepo = basketItemRepo;
            _productRepo = productRepo;
        }
        public Task<Basket> AddItemToBasketAsync(string buyerId, int productId, int quantity)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteBasketItemAsync(string buyerId, int productId)
        {
            var basket = await GetOrCreateBasketAsync(buyerId);
            var basketItem = basket.Items.FirstOrDefault(x => x.ProductId == productId);
            if (basketItem == null) return;
            await _basketItemRepo.DeleteAsync(basketItem);
        }

        public async Task EmptyBasketAsync(string buyerId)
        {
            var basket = await GetOrCreateBasketAsync(buyerId);

            foreach (var item in basket.Items.ToList())
            {
                await _basketItemRepo.DeleteAsync(item);
            }
        }

        public async Task<Basket> GetOrCreateBasketAsync(string buyerId)
        {
            var specBasket = new BasketWithItemsSpecification(buyerId);
            var basket = await _basketRepo.FirsOrDefaultAsync(specBasket);

            if (basket == null)
            {
                basket = new Basket() { BuyerId = buyerId };
                await _basketRepo.AddAsync(basket);
            }

            return basket;
        }
        public async Task<Basket> SetQuantities(string buyerId, Dictionary<int, int> quantities)
        {
            var basket = await GetOrCreateBasketAsync(buyerId);

            foreach (var item in basket.Items)
            {
                if (quantities.ContainsKey(item.Id))
                {
                    item.Quantity = quantities[item.ProductId];
                    await _basketItemRepo.UpdateAsync(item);
                }
            }

        }

        public Task TransferBasketAsync(string sourceBuyerId, string destinationBuyerId)
        {
            throw new NotImplementedException();
        }
    }
}
