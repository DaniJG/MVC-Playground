using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DJRM.Common.Infrastructure.Profiling
{
    public class Cookies
    {
        public static string Glimpse = "glimpsePolicy";
        public static string MiniProfiler = "enableProfiler";

        public static bool MiniProfilerEnabled(HttpRequest request)
        {
            return CookieEnabled(Cookies.MiniProfiler, request.Cookies);
        }

        public static bool MiniProfilerEnabled(HttpRequestBase request)
        {
            return CookieEnabled(Cookies.MiniProfiler, request.Cookies);
        }

        public static bool GlimpseEnabled(HttpRequest request)
        {
            return CookieEnabled(Cookies.Glimpse, request.Cookies);
        }

        public static bool GlimpseEnabled(HttpRequestBase request)
        {
            return CookieEnabled(Cookies.Glimpse, request.Cookies);
        }

        private static bool CookieEnabled(string cookie, HttpCookieCollection cookies)
        {
            return cookies[cookie] != null && cookies[cookie].Value == "On";
        }
    }
}