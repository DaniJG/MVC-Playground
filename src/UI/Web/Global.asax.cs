using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using StackExchange.Profiling;

namespace DJRM.UI.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            MiniProfilerConfig.InitializeProfiling();
            UnityBootstrapper.Initialise();
        }

        protected void Application_BeginRequest()
        {
            //Enable mini profiler when debugging is enabled. We could also have something in the web.config
            if (Request.RequestContext.HttpContext.IsDebuggingEnabled 
                && DJRM.Common.Infrastructure.Profiling.Cookies.MiniProfilerEnabled(Request))
            {
                MiniProfiler.Start();
            }
        }

        protected void Application_EndRequest(object sender, System.EventArgs e)
        {
            //Stop profiling
            MiniProfiler.Stop(); //stop as early as you can, even earlier with MvcMiniProfiler.MiniProfiler.Stop(discardResults: true);
        }
    }
}