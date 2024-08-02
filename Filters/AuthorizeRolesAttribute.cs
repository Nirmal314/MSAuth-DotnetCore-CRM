using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace MSAuth.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AuthorizeRolesAttribute : TypeFilterAttribute
{
    public AuthorizeRolesAttribute(params string[] roles) : base(typeof(AuthorizeRolesFilter))
    {
        Arguments = [roles];
    }
}
