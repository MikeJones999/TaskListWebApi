using System.Net;

namespace TaskList.Api.Application.ExceptionHandling.CustomHandler
{
    public class JwtTokenValidationException : BaseException
    {
        public JwtTokenValidationException(string message = "") : base($"Validation of Jwt failed. {message}. Unable to continue", HttpStatusCode.Unauthorized)
        {
        }
    }
}
