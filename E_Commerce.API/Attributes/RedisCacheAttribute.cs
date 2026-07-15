using E_Commerce.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace E_Commerce.API.Attributes
{
    public class RedisCacheAttribute : ActionFilterAttribute
    {
        private readonly int _durationInSec;

        public RedisCacheAttribute(int durationInSec = 60)
        {
            _durationInSec = durationInSec;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get Cache Service From Container []
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var cacheKey = CreateCacheKey(context.HttpContext.Request);
            var data = await cacheService.GetDataAsync(cacheKey);
            // If Data Exists in Cache => Get Data from cache + skip endpoint
            if(!string.IsNullOrEmpty(data))
            {
                context.Result = new ContentResult()
                {
                    Content = data,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;
            }
            // If Data Not Exists in Cache => Execute endpoint + Store result in cache If Result is 200OK + Data
            var executedContext = await next.Invoke();
            if(executedContext.Result is OkObjectResult { Value: not null } ok)
            {
                await cacheService.SetDataAsync(cacheKey, ok.Value, TimeSpan.FromSeconds(_durationInSec));
            }
        }

        private static string CreateCacheKey(HttpRequest request)
        {
            var key = new StringBuilder();
            key.Append(request.Path);
            if(request.Query.Any())
            {
                key.Append('?');
                foreach(var (k,v) in request.Query.OrderBy(x=>x.Key))
                {
                    key.Append(k).Append('=').Append(v).Append('&');
                }
            }
            return key.ToString();
        }
    }
}
