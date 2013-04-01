using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DJRM.Common.Controllers.ErrorHandling
{
    //Tribute to Atif Aziz: http://stackoverflow.com/questions/766610/how-to-get-elmah-to-work-with-asp-net-mvc-handleerror-attribute
    public class ElmahErrorHandler : ElmahErrorHandlerBase
    {
        public override void OnException(ExceptionContext context)
        {
            //When already handled, do nothing
            if (context.ExceptionHandled)
                return;

            //Run the base functionality
            base.OnException(context);

            //If the base functionaility didnt handled the exception, then exit (as the exception is not of the type this filter should handle)
            if (!context.ExceptionHandled)
                return;

            //Check if this is an Ajax request, if so make sure a partial view result is returned
            if (context.HttpContext.Request.IsAjaxRequest() && !(context.Result is PartialViewResult))
            {
                context.Result = GetPartialViewResult(context);
            }

            //The excepcion is logged
            LogException(context);
        }
    }
}