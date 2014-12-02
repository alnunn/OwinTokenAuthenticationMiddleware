using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwinTokenAuthenticationMiddleware
{
    public class TokenManager : ITokenManager
    {
        IAccessTokenObtainer ValidTokenObtainer;
        static List<AccessToken> Tokens;
        CacheExpiration CacheTimer;


        public TokenManager(IAccessTokenObtainer ValidTokenObtainer, int RefreshCacheInMinutes)
        {
            this.ValidTokenObtainer = ValidTokenObtainer;
            CacheTimer = new CacheExpiration(RefreshCacheInMinutes, RefreshTokens);

            Tokens = GetAllCurrentTokens();
        }

        public bool IsValidToken(AccessToken Token)
        {
            if(Tokens.Contains(Token))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<AccessToken> GetAllCurrentTokens()
        {
            Console.WriteLine("Getting all tokens");
            return ValidTokenObtainer.GetAccessTokens();
        }

        private void RefreshTokens(DateTime UpdateTime)
        {
            Tokens = GetAllCurrentTokens();
        }



    }
}
