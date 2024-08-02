using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace MSAuth.Claims
{
    public class CustomClaimsTransformer(ICrmService crmService) : IClaimsTransformation
    {
        private readonly ICrmService _crmService = crmService;

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity identity = (ClaimsIdentity)principal.Identity!;
            var userId = identity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (userId != null && !identity.Claims.Any(c => c.Type == ClaimTypes.Role))
            {
                var roles = await _crmService.GetUserRolesFromCrm(userId);

                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }

            return principal;
        }
    }
}
