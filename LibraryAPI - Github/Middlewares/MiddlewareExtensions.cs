namespace LibraryAPI.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTimingMiddleware>();  // program.cs de yer alan middleware hattına ekler ve http isteklerinin bu middlewaredne geçmesini sağlar
        }
    }
}