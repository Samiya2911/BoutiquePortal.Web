using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BoutiquePortal.Web.Filters
{
    public class CustomerAuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");

            if (role != "Customer")
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "CustomerAccount",
                    new { area = "Customer" }
                );
            }

            base.OnActionExecuting(context);
        }
    }
}