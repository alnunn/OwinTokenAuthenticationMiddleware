using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwinTokenAuthenticationMiddleware
{
    public class AccessToken : IEquatable<AccessToken>
    {
        public AccessToken()
        {

        }

        public string Token { get; set; }

        public bool Equals(AccessToken other)
        {
            if (this.Token == other.Token)
            {
                return true;
            }

            return false;
        }
    }
}
