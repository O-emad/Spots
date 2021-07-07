using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Base
{
    public class BaseController : Controller
    {
        public Guid VendorId { get; set; }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.IsInRole("Vendor"))
            {
                var vendorId = User.Claims.FirstOrDefault(c => c.Type == "vendor")?.Value;
                if (!string.IsNullOrWhiteSpace(vendorId))
                {
                    Guid vendorIdGuid;
                    if(Guid.TryParse(vendorId,out vendorIdGuid))
                    {
                        VendorId = vendorIdGuid;
                        //if (context.HttpContext.Request.RouteValues.ContainsKey("id"))
                        //{
                        //    context.HttpContext.Request.RouteValues["id"] = vendorIdGuid;
                        //    var controller = context.HttpContext.Request.RouteValues["controller"]?.ToString();
                        //    var action = context.HttpContext.Request.RouteValues["action"]?.ToString();
                        //    var url = Url.Action(action, controller, vendorIdGuid);
                        //    context.ActionArguments["id"] = vendorIdGuid;
                        //    context.RouteData.
                        //    context.HttpContext.Request.Path = url;
                        //}
                    }
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
