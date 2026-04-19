using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BoutiquePortal.Web.Filters
{
    public class AdminAuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                context.Result = new RedirectToActionResult(
                    "Login",        // Action
                    "AdminAccount", // Controller
                    new { area = "Admin" }
                );
            }

            base.OnActionExecuting(context);
        }
    }   
}
