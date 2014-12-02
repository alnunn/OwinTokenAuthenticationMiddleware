using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OwinTokenAuthenticationMiddleware
{
    public class CacheExpiration 
    {
        Timer StopWatch;
        List<Action<DateTime>> MethodsToCall = new List<Action<DateTime>>();

        public CacheExpiration(int TimeToRefresh, Action<DateTime> Method)
        {
            int TimeToRefreshInMinutes = TimeToRefresh * 1000 * 60;
            MethodsToCall.Add(Method);
            StopWatch = new Timer(TimeToRefreshInMinutes);
            StopWatch.Elapsed += TimerFired; 
            
            StopWatch.Enabled = true;
        }

        private void TimerFired(object o, ElapsedEventArgs args)
        {
            foreach(var Method in MethodsToCall)
            {
                Method(DateTime.Now);
            }
        }
    }
}
