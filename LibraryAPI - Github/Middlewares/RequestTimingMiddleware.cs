using System.Diagnostics;

namespace LibraryAPI.Middlewares
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next; // bir sonraki middleware'e geçmeyi sağlar
        private readonly ILogger<RequestTimingMiddleware> _logger; // loglama yapmak için kullanılır, bu logger yazdığımız middleware'in çalışmasını takip etmemizi sağlar

        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger) // dependency injection
        {
            _next = next;  // gelen parametreleri sınıf değişkenlerine atar
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) // tüm http istekleri burdan  geçer
        {
            var stopwatch = Stopwatch.StartNew(); // süre ölçümünü başlatır

            await _next(context); // bir sonraki middleware'e geçer

            stopwatch.Stop(); // süreyi durdur

            var elapsedMs = stopwatch.ElapsedMilliseconds; // geçen süre

            var method = context.Request.Method; // istek hangi http metodu ile yapıldığını gösterir
            var path = context.Request.Path; // isteğin url'i

            _logger.LogInformation($"[RequestTiming] {method} {path} → {elapsedMs}ms sürdü.");  // middleware sonuç bilgisi
        }
    }
}
