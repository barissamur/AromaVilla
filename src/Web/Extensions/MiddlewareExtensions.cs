using Web.MiddleWares;

namespace Web.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void UseTransferBasket(this IApplicationBuilder appplicationBuilder)
        {
            appplicationBuilder.UseMiddleware<TransferBasketMiddleware>();
        }
    }
}
