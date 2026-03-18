using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace ISH_APP.Filtres
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Session.GetString("UtilisateurRole");

            if (string.IsNullOrEmpty(role) || !_roles.Contains(role))
            {
                context.Result = new RedirectToPageResult("/AccessDenied");
            }
        }
    }
}
