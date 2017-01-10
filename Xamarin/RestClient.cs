using System;
using System.IO;
using System.Net;
using System.Text;
using XamarinApp.Common;
using Xamarin.Forms;

namespace XamarinApp.RestServices
{
    public class RestClient : ViewModelBase
    {
        private const string customAuthTokenHeaderName = "AuthToken";
        private const string customeIPAddressHeaderName = "ClientIPAddress";

        public HttpVerb Method { get; set; }
        public string EndPoint { get; set; }
        public string ContentType { get; set; }
        public string Data { get; set; }
        public bool IncludeAuthTokenAndIPAddress { get; set; }

        public RestClient(string endpoint, HttpVerb method, string data, string contentType, bool includeAuthTokenAndIPAddress)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = contentType;
            Data = data;
            IncludeAuthTokenAndIPAddress = includeAuthTokenAndIPAddress;
        }

        public RestResponse SendRequest()
        {
            return SendRequest("");
        }

        public RestResponse SendRequest(string parameters)
        {
            RestResponse restResponse = new RestResponse();
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);
            request.Method = Method.ToString();
            request.ContentType = ContentType;
            var timeout = 30000;
            request.Timeout(timeout);

            if (IncludeAuthTokenAndIPAddress)
            {
                request.Headers[customAuthTokenHeaderName] = AppVM.DataTransporter.MdtAuthToken.Token;
                request.Headers[customeIPAddressHeaderName] = AppVM.CellularMon.IPaddress;
            }

            try
            {
                // If POST or PUT send data
                if ((Method == HttpVerb.POST || Method == HttpVerb.PUT) && !string.IsNullOrEmpty(Data))
                {
                    var bytes = Encoding.GetEncoding(Xapp.GetResource("RM_EncodingStandard") as string).GetBytes(Data);
                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }

                // Get response
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    restResponse.HttpStatus = response.StatusCode;
                    restResponse.HttpStatusDescription = response.StatusDescription;
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                restResponse.Data = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    restResponse.HttpStatus = response.StatusCode;
                    restResponse.HttpStatusDescription = response.StatusCode == HttpStatusCode.Accepted ? "Unknown internal error occurred. See server logs for more information" : response.StatusDescription;
                }
                else
                {
                    restResponse.HttpStatus = HttpStatusCode.InternalServerError;
                    restResponse.HttpStatusDescription = $"Unknown Internal Error: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                restResponse.HttpStatus = HttpStatusCode.InternalServerError;
                restResponse.HttpStatusDescription = $"Unknown Internal Error: {ex.Message}";
            }

            return restResponse;
        }
    }
}

public enum HttpVerb
{
    GET,
    POST,
    PUT,
    DELETE
}

