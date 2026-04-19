using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BoutiquePortal.Web.Filters
{
    public class VendorAuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");

            if (role != "Vendor")
            {
                context.Result = new RedirectToActionResult(
                    "Login",         // Action
                    "VendorAccount", // Controller
                    new { area = "Vendor" }
                );
            }

            base.OnActionExecuting(context);
        }
    }
}