using System.Web;
using System.Web.Mvc;
using StackExchange.Profiling.MVCHelpers;
using DJRM.Common.Controllers.ErrorHandling;

namespace DJRM.UI.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ElmahErrorHandler());
            filters.Add(new ProfilingActionFilter());
        }
    }
}