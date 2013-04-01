using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StackExchange.Profiling;
using Microsoft.Practices.Unity.InterceptionExtension;
using DJRM.Common.Infrastructure;

namespace DJRM.Common.Infrastructure.Profiling
{
    public class ProfilerInterceptor : IInterceptionBehavior
    {
        /// <summary>
        /// Returns the interfaces required by the behavior for the objects it intercepts.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        /// <summary>
        /// Wraps the invocation of the method inside a mini-profiler step
        /// </summary>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var profiler = MiniProfiler.Current; // it's ok if this is null
            var stepName = String.Empty;
            if (profiler != null)//check for null, but just to avoid overhead
            {
                stepName = GetStepName(input);
            }
            using (profiler.Step(stepName))//if null its not a proble, "Step" is an extension method
            {
                //Call method in the intercepted component
                var ret = getNext().Invoke(input, getNext);
                //Return same result from the intercepted component
                return ret;
            }
        }

        private string GetStepName(IMethodInvocation input)
        {
            string args = String.Empty;
            for (int x = 0; x < input.Arguments.Count; x++)
            {
                string argumentValue = "" + input.Arguments[x];
                if (input.Arguments[x] is EntityBase)
                {
                    argumentValue = (input.Arguments[x] as EntityBase).Id + "-" + (input.Arguments[x] as EntityBase).ObjectType.Name;
                }
                string argument = string.Format("{0}:{1}", input.Arguments.ParameterName(x), argumentValue);
                args += (args == String.Empty ? argument : "," + argument);
            }
            return String.Format("{0}.{1}({2})", input.Target.GetType().Name, input.MethodBase.Name, args);
        }

        public bool WillExecute
        {
            get
            {
                return true;
            }
        }
    }
}