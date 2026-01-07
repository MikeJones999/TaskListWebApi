namespace TaskListWebApi.Middleware
{
    public static class JwtMiddlewareRoutes
    {
        public const string Login = "/api/user/login";
        public const string Register = "/api/user/register";

        public const string Authorisation = "Authorization";
        public const string Bearer = "Bearer ";

        //combine these into an object
        public const string UserId = "UserId";
        public const string UserName = "UserName";

        public static string[] GetListOfPathsToIgnore()
        {
            return [Login, Register];
        }
    }
}
