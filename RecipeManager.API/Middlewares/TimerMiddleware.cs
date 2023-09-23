using System.Globalization;

namespace RecipeManager.API.Middlewares
{
    public class TimerMiddleware
    {
        private readonly RequestDelegate _next;
        public TimerMiddleware(RequestDelegate next)
        {
                _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            Thread.Sleep(500); // mimic server slowness

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }
}
