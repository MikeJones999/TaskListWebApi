using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Api.Application.ExceptionHandling.CustomHandler;
using TaskListWebApi.Middleware;

namespace TaskListWebApi.Controllers
{
    [Authorize]
    [ApiController]
    public class BaseAuthController : ControllerBase
    {
        protected readonly ILogger<BaseAuthController> _logger;

        public BaseAuthController(ILogger<BaseAuthController> logger)
        {
            _logger = logger;
        }

        protected Guid UserId => Guid.Parse(ExtractKey(JwtMiddlewareRoutes.UserId));
        protected string UserName => ExtractKey(JwtMiddlewareRoutes.UserName);

        protected void ValidateUserPassedInAgainstAuth(Guid userId, string methodName)
        {
            if (userId == Guid.Empty || userId != UserId)
            {
                _logger.LogWarning(message: "Request for User profile failed - userId Guid not found. Request {Method}", methodName);
                throw new UserNotFoundException(userId, methodName);
            }
        }

        //protected static void UpdateResponse<T>(ResponseDto<T> externalResponse, string message = "", bool status = false, T? data = default)
        //{
        //    externalResponse.Success = status;
        //    externalResponse.Message = message;
        //    externalResponse.ResponseData = data;
        //}

        private string ExtractKey(string key)
        {
            return HttpContext.Items[key]?.ToString()!;
        }

    }
}
