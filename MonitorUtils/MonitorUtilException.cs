using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorUtils
{
    class MonitorUtilException : Exception
    {
        public MonitorUtilException(string message)
            : base(message)
        {

        }

        public MonitorUtilException(string message, Exception exception) 
            : base(message, exception)
        {

        }
    }
}
