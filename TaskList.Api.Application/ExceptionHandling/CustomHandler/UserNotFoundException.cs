using System.Net;

namespace TaskList.Api.Application.ExceptionHandling.CustomHandler
{
    public class UserNotFoundException : BaseException
    {
        public UserNotFoundException(Guid userId, string methodName) : base($"User with id {userId} not found at {methodName}", HttpStatusCode.NotFound)
        {
        }
    }
}
