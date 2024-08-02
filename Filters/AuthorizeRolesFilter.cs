using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MSAuth.Interfaces;
using MSAuth.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MSAuth.Filters;

public class AuthorizeRolesFilter : IAuthorizationFilter
{
    private readonly string[] _roles;
    private readonly ServiceClient _service;
    private readonly IEmailService _emailService;

    public AuthorizeRolesFilter(ServiceClient service, IEmailService emailService, string[] roles)
    {
        _roles = roles;
        _service = service;
        _emailService = emailService;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        ISession? session = context.HttpContext?.Session;
        string? rolesStored = session?.GetString("Roles");
        string? userId = session?.GetString("UserId");

        string sIsFirstLogin = session?.GetString("IsFirstLogin")!;


        if (rolesStored == null)
        {
            var user = context.HttpContext?.User;
            if (user?.Identity != null && user.Identity.IsAuthenticated)
            {
                var userIdentity = (ClaimsIdentity)user.Identity;
                var claims = userIdentity.Claims;
                var roles = claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

                if (roles.Count > 0)
                {
                    context.HttpContext?.Session.SetString("Roles", string.Join(",", roles));
                }
            }
        }

        if (userId == null)
        {
            ClaimsPrincipal user = context.HttpContext?.User!;

            if (user?.Identity != null && user.Identity.IsAuthenticated)
            {
                ClaimsIdentity identity = (ClaimsIdentity)user.Identity!;

                string? azureId = identity.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

                if (azureId != null)
                {
                    var query = new QueryExpression("systemuser")
                    {
                        ColumnSet = new ColumnSet("systemuserid")
                    };
                    query.Criteria.AddCondition("azureactivedirectoryobjectid", ConditionOperator.Equal, azureId);

                    string systemUserId = _service.RetrieveMultiple(query)[0].Id.ToString();

                    if (systemUserId != null)
                    {
                        context.HttpContext?.Session.SetString("UserId", systemUserId);
                    }
                }

            }
        }

        if (string.IsNullOrEmpty(sIsFirstLogin) || sIsFirstLogin == "Yes")
        {
            Entity result = _service.Retrieve("systemuser", new Guid(session?.GetString("UserId")!), new ColumnSet("cr267_isfirstlogin"));
            bool isFirstLogin = result.GetAttributeValue<bool>("cr267_isfirstlogin");

            if (isFirstLogin || isFirstLogin as bool? == null)
            {
                var subject = "Welcome to Our Application!";
                var message = $"Hello {context.HttpContext?.User.Claims.First(c => c.Type == "preferred_username").Value!},\n\nThank you for logging in for the first time!";

                _emailService.SendEmail(context.HttpContext?.User.Identity?.Name!, subject, message);

                Entity record = new("systemuser", new Guid(session?.GetString("UserId")!));
                record["cr267_isfirstlogin"] = false;
                _service.Update(record);

                session?.SetString("IsFirstLogin", "No");
            }
        }

        string? rolesString = session?.GetString("Roles");

        if (rolesString != null)
        {
            string[]? userRoles = rolesString.Split(',');

            if (!_roles.Any(role => userRoles.Contains(role)))
            {
                context.Result = new ForbidResult();
            }
        }
        else
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
