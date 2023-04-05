namespace Web.MiddleWares
{
    public class TransferBasketMiddleware
    {
        private readonly RequestDelegate _next;

        public TransferBasketMiddleware(RequestDelegate next) // sıradaki middleware yi al
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IBasketViewModelService basketViewModelService)
        {
            await basketViewModelService.TransferBasketAsyc();
            await _next(context);
        }
    }
}
