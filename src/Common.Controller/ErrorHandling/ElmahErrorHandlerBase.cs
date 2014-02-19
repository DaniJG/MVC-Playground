using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using Elmah;

namespace DJRM.Common.Controllers.ErrorHandling
{
    public class ElmahErrorHandlerBase : HandleErrorAttribute
    {
        protected PartialViewResult GetPartialViewResult(ExceptionContext context)
        {
            string controllerName = (string)context.RouteData.Values["controller"];
            string actionName = (string)context.RouteData.Values["action"];
            HandleErrorInfo model = new HandleErrorInfo(context.Exception, controllerName, actionName);
            var result = new PartialViewResult
            {
                ViewName = View,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = context.Controller.TempData
            };
            return result;
        }

        protected void LogException(ExceptionContext context)
        {
            //Check when this needs to be logged on Elmah
            var e = context.Exception;
            if (!context.ExceptionHandled   // if unhandled, will be logged anyhow                 
                || RaiseErrorSignal(e)      // prefer signaling, if possible                 
                || IsFiltered(context))     // filtered?                 
                return;
            LogException(e);
        }

        private static bool RaiseErrorSignal(Exception e)
        {
            var context = HttpContext.Current;
            if (context == null)
                return false;

            var signal = ErrorSignal.FromContext(context);

            if (signal == null)
                return false;

            signal.Raise(e, context); return true;
        }

        private static bool IsFiltered(ExceptionContext context)
        {
            var config = context.HttpContext.GetSection("elmah/errorFilter") as ErrorFilterConfiguration;
            if (config == null)
                return false;

            var testContext = new ErrorFilterModule.AssertionHelperContext(context.Exception, HttpContext.Current);

            return config.Assertion.Test(testContext);
        }

        private static void LogException(Exception e)
        {
            var context = HttpContext.Current;
            ErrorLog.GetDefault(context).Log(new Error(e, context));
        }
    }
}
