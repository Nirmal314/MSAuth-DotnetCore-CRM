namespace MSAuth.Services
{
    public class SessionService(IHttpContextAccessor httpContextAccessor)
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public string GetUserRoles()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("UserRoles")!;
        }
    }
}
