using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StackExchange.Profiling;
using StackExchange.Profiling.MVCHelpers;

namespace DJRM.UI.Web
{
    public class MiniProfilerConfig
    {
        /// <summary>
        /// Initialize mini-profiler. See http://code.google.com/p/mvc-mini-profiler/source/browse/Sample.Mvc/ for an example
        /// </summary>
        public static void InitializeProfiling()
        {
            //initialize automatic view profiling
            var copy = ViewEngines.Engines.ToList();
            ViewEngines.Engines.Clear();
            foreach (var item in copy)
            {
                ViewEngines.Engines.Add(new ProfilingViewEngine(item));
            }

            //MiniProfiler for EF
            MiniProfilerEF.Initialize(false);

            //Profiling data can be saved to a database.
            //Sample project also contains a SqlLiteServerStorage that creates an SQLLite file each time
            //string connectionString = ... ;
            //MiniProfiler.Settings.Storage = new StackExchange.Profiling.Storage.SqlServerStorage(connectionString);

            MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();
            MiniProfiler.Settings.MaxJsonResponseSize = 2097152; //default 4MB(2097152)
        }
    }
}