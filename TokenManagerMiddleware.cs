using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin;
using System.IO;

namespace OwinTokenAuthenticationMiddleware
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class TokenManagerMiddleware
    {
        AppFunc NextFunction;
        static ITokenManager AccessTokenManager;
        static IAccessTokenObtainer TokenObtainer;

        public TokenManagerMiddleware(AppFunc _NextFunction, IAccessTokenObtainer AccessTokenObtainer, int CacheRefreshTimeInMinutes)
        {
            NextFunction = _NextFunction;
            TokenObtainer = AccessTokenObtainer;
            AccessTokenManager = new TokenManager(TokenObtainer, CacheRefreshTimeInMinutes);
        }


        public Task Invoke(IDictionary<string, object> Environment)
        {
            try
            {
                var Headers = (IDictionary<string, string[]>)Environment["owin.RequestHeaders"];


                if (Headers.ContainsKey("access_token"))
                {
                    if (AccessTokenManager.IsValidToken(TokenFromHttpHeaders(Headers)))
                    {
                        return NextFunction(Environment);
                    }
                    else
                    {
                        var ResponseData = new Response();
                        ResponseData.Message = "Invalid Token Supplied";
                        ResponseData.Environment = Environment;
                        ResponseData.ResponseCode = 401;
                        var NewHeader = new Dictionary<string, string[]>();
                        NewHeader.Add("WWW-Authenticate",new string[]{"Supplied token not valid"});
                        ResponseData.AppendHeaders = NewHeader;

                        return ReturnResponse(ResponseData);
                    }
                }
                else
                {
                    var ResponseData = new Response();
                    ResponseData.Message = "Invalid Token Supplied";
                    ResponseData.Environment = Environment;
                    ResponseData.ResponseCode = 401;
                    var NewHeader = new Dictionary<string, string[]>();
                    NewHeader.Add("WWW-Authenticate", new string[] { "You must supply header access_token"});
                    ResponseData.AppendHeaders = NewHeader;
                    return ReturnResponse(ResponseData);
                }
            }
            catch(Exception ex)
            {
                StringBuilder Message = new StringBuilder();
                Message.AppendLine(ex.Message);
                Message.AppendLine(ex.StackTrace);
                Message.AppendLine(ex.Source);
                System.Windows.Forms.MessageBox.Show(Message.ToString());

                var ResponseData = new Response();
                ResponseData.Message = "Exception caught processing token authentication";
                ResponseData.Environment = Environment;
                ResponseData.ResponseCode = 500;
                return ReturnResponse(ResponseData);
 
            }
            finally
            {
                TokenObtainer = null;
            }
        }

        private Task ReturnResponse(Response ResponseData)
        {
            var Headers = (IDictionary<string, string[]>)ResponseData.Environment["owin.RequestHeaders"];
            foreach(KeyValuePair<string, string[]> KeyAndValue in ResponseData.AppendHeaders)
            {
                Headers.Add(KeyAndValue.Key, KeyAndValue.Value);
            }

            ResponseData.Environment["owin.ResponseStatusCode"] = ResponseData.ResponseCode;
            var ResponseStream = (Stream)ResponseData.Environment["owin.ResponseBody"];
            StreamWriter Writer = new StreamWriter(ResponseStream);
            return Writer.WriteAsync(ResponseData.Message);
        }

        private AccessToken TokenFromHttpHeaders(IDictionary<string, string[]> Headers)
        {
            var Token = new AccessToken();
            Token.Token = Headers["access_token"][0];
            return Token;            
        }

    }

    public static partial class AppBuilderExtensions
    {
        public static void UseAccessTokenManagement(this IAppBuilder App, IAccessTokenObtainer TokenObtainer, int CacheRefreshInMinutes)
        {
            App.Use<TokenManagerMiddleware>(TokenObtainer, CacheRefreshInMinutes);
        }
    }
}
