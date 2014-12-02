using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwinTokenAuthenticationMiddleware
{
    internal class Response
    {
        public int ResponseCode { get; set; }
        public string Message {get;set;}
        public IDictionary<string, string[]> AppendHeaders { get; set; }
        public IDictionary<string, object> Environment { get; set; }
    }
}
