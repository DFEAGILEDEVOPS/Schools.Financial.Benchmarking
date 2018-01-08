using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SFB.Web.Domain.ApiWrappers;

namespace SFB.Web.Domain
{
    public class ApiRequest : IApiRequest
    {
        string _baseUrl;
        string _username;
        string _password;

        public ApiRequest(string baseUrl) : this(baseUrl, string.Empty, string.Empty) {}
        
        public ApiRequest(string baseUrl, string userName, string password)
        {
            _baseUrl = baseUrl;
            _username = userName;
            _password = password;
        }

        public ApiResponse Get(string endpoint, List<string> actions, Dictionary<string, string> parameters)
        {
            var url = BuildEndpointUrl(endpoint, actions, parameters);	

            WebRequest request = WebRequest.Create(url);

            SetBasicAuthHeader(request);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            Stream dataStream = response.GetResponseStream();
            
            StreamReader reader = new StreamReader(dataStream);
            
            string responseFromServer = reader.ReadToEnd();

            var responseObject = JsonConvert.DeserializeObject(responseFromServer, typeof(ExpandoObject));

            reader.Close();
            dataStream.Close();
            response.Close();

            return new ApiResponse(response.StatusCode, responseObject);
        }

        public ApiResponse Get(string endpoint, List<string> parameters)
        {
            var url = BuildEndpointUrl(endpoint, parameters);

            WebRequest request = WebRequest.Create(url);

            SetBasicAuthHeader(request);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseFromServer);

            reader.Close();
            dataStream.Close();
            response.Close();

            return new ApiResponse(response.StatusCode, responseObject);
        }

        public async Task<ApiResponse> GetAsync(string endpoint, List<string> actions, Dictionary<string, string> parameters)
        {
            var url = BuildEndpointUrl(endpoint, actions, parameters);

            WebRequest request = WebRequest.Create(url);

            SetBasicAuthHeader(request);

            WebResponse response;
            try
            {
                response = await request.GetResponseAsync();
            }
            catch (Exception)
            {

                return new ApiResponse(HttpStatusCode.InternalServerError, null);
            }            

            Stream dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseFromServer);

            reader.Close();
            dataStream.Close();
            response.Close();

            return new ApiResponse(HttpStatusCode.OK, responseObject);
        }

        private string BuildEndpointUrl(string endpoint, List<string> actions, Dictionary<string, string> parameters)
        {
            var url = String.Format("{0}/{1}" , _baseUrl, endpoint);

            foreach (var action in actions)
            {
                url += "/" + action;
            }

            var concatenatedParams = string.Empty;
            foreach (var parameter in parameters)
            {
                concatenatedParams = $"{concatenatedParams}{(concatenatedParams.Length == 0 ? "?" : "&")}{parameter.Key}={parameter.Value}";
            }

            url += concatenatedParams.TrimEnd('&');

            return url;
        }

        private string BuildEndpointUrl(string endpoint, List<string> parameters)
        {
            var url = String.Format("{0}/{1}/", _baseUrl, endpoint);

            foreach (var parameter in parameters)
            {
                url += String.Format("{0},", parameter);
            }

            url = url.TrimEnd(',');

            return url;
        }

        private void SetBasicAuthHeader(WebRequest request)
        {
            if (_username != string.Empty && _password != string.Empty)
            {
                string authInfo = _username + ":" + _password;
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                request.Headers["Authorization"] = "Basic " + authInfo;
            }
        }
    }
}
