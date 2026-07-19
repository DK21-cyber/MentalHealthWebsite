using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PW.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public SessionAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(role))
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Account",
                    null);

                return;
            }

            if (_roles.Length > 0 && !_roles.Contains(role))
            {
                context.Result = new RedirectToActionResult(
                    "AccessDenied",
                    "Account",
                    null);

                return;
            }

            base.OnActionExecuting(context);
        }
    }
}