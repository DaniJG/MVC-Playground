using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DJRM.Common.Controllers.ErrorHandling
{
    public class ElmahAJAXErrorHandler : ElmahErrorHandlerBase
    {
        public override void OnException(ExceptionContext context)
        {

            //This code is copied from the HandleError attribute that comes with ASP MVC 3, as if we just derive from it
            //and call base.OnException(context), the attribute does nothing because the MVC HandleError attribute is explicitly excluding child requests

            //When already handled, do nothing
            if (context.ExceptionHandled)
                return;

            // If custom errors are disabled, we need to let the normal ASP.NET exception handler
            // execute so that the user can see useful debugging information.
            if (context.ExceptionHandled || !context.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            Exception exception = context.Exception;

            // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method),
            // ignore it.
            if (new HttpException(null, exception).GetHttpCode() != 500)
            {
                return;
            }

            if (!ExceptionType.IsInstanceOfType(exception))
            {
                return;
            }

            // Build a partial view result. This bit is different from the HandleError attribute, where a standard view result is created
            context.Result = GetPartialViewResult(context);

            //Set the properties to the response object, same as the HandleError attribute (the last bit about TrySkipIisCustomErrors flag is important)
            context.ExceptionHandled = true;
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.StatusCode = 500;
            // Certain versions of IIS will sometimes use their own error page when
            // they detect a server error. Setting this property indicates that we
            // want it to try to render ASP.NET MVC's error page instead.
            context.HttpContext.Response.TrySkipIisCustomErrors = true;


            //The excepcion is logged
            LogException(context);
        }
    }
}