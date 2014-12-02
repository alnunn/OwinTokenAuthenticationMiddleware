using System;


namespace OwinTokenAuthenticationMiddleware
{
    public interface ITokenManager
    {
        bool IsValidToken(AccessToken Token);
    }
}
