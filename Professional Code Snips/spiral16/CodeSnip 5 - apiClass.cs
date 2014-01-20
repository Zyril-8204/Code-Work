using Web.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace API.Twitter
{
    internal class ApiRequest
    {
        public static string RequireSocialID(string url, iSocialAccount socialUser, params object[] formats)
        {
            return RequireSocialID(url, null, socialUser, formats);
        }
        public static string RequireSocialID(string url, string postData, iSocialAccount socialUser, params object[] formats)
        {
            if (!string.IsNullOrEmpty(socialUser.SocialID))
            {
                return RequireCredentials(url, postData, socialUser, formats);
            }
            else
            {
                throw new Exception.ImplementationException("SocialID can't be null.\n\nPlease verify that you are passing a valid SocialID; the SocialID should be the PageID in this case.");
            }
        }

        public static string RequireCredentials(string url, iSocialAccount socialUser, params object[] formats)
        {
            return RequireCredentials(url, null, socialUser, formats);
        }
        public static string RequireCredentials(string url, string postData, iSocialAccount socialUser, params object[] formats)
        {
            if (!string.IsNullOrEmpty(socialUser.AccessToken))
            {
                if (!string.IsNullOrEmpty(socialUser.AccessTokenSecret))
                {
                    return HandleApiExceptions(url, postData, socialUser, formats);
                }
                else
                {
                    throw new Exception.ImplementationException("AccessTokenSecret can't be null.\n\nPlease verify that you are passing a valid AccessTokenSecret.");
                }
            }
            else
            {
                throw new Exception.ImplementationException("AccessToken can't be null.\n\nPlease verify that you are passing a valid AccessToken.");
            }
        }

        public static string HandleApiExceptions(string url, iSocialAccount socialUser, params object[] formats)
        {
            return HandleApiExceptions(url, null, socialUser, formats);
        }
        public static string HandleApiExceptions(string url, string postData, iSocialAccount socialUser, params object[] formats)
        {
            try
            {
                url = url.FormatWith(socialUser);
                url = string.Format(url, formats);
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.CreateDefault(new Uri(url));

                if (!string.IsNullOrEmpty(postData))
                {
                    postData = postData.FormatWith(socialUser);
                    postData = string.Format(postData, formats);


                    return req.GetResponseString(postData, socialUser.ConsumerKey, socialUser.ConsumerSecret, socialUser.AccessToken, socialUser.AccessTokenSecret);
                }

                return req.GetResponseString(socialUser.ConsumerKey, socialUser.ConsumerSecret, socialUser.AccessToken, socialUser.AccessTokenSecret);
            }
            catch (ProtocolException pex)
            {
                switch (pex.StatusCode)
                {
                    case ApiStatusCode.BadRequest:
                        throw new Exception.ApiException("Invalid Credentials");
                    case ApiStatusCode.TooManyRequest:
                        throw new Exception.ApiException("Exceeded API limits");
                    default:
                        throw pex;
                }
                throw pex;
            }
        }
    }
}
