using LeanCloud.Storage.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
    /// <summary>
    /// an action from Player will be packed as a command sent to LeanCloud Game server.
    /// </summary>
    public class PlayCommand
    {

        /// <summary>
        /// play command
        /// </summary>
        public PlayCommand()
        {

        }

        internal readonly string GameRouterSchema = "https";
        internal readonly string GameRouterHost = "game-router-cn-e1.leancloud.cn";
        internal readonly string ApiVersion = "v1";
        internal static IDictionary<string, object> headers;
        public static IDictionary<string, object> Headers
        {
            get
            {
                if (headers == null)
                {
                    headers = new Dictionary<string, object>();
                    headers.Add(new KeyValuePair<string, object>("X-LC-ID", AVClient.CurrentConfiguration.ApplicationId));
                    headers.Add(new KeyValuePair<string, object>("Content-Type", "application/json"));
                }
                return headers;
            }
        }
        const string SESSION_TOKEN_KEY = "X-LC-Play-Session-Token";
        internal static void SetAuthentication(string sessionToken)
        {
            if (!Headers.ContainsKey(SESSION_TOKEN_KEY))
            {
                headers.Add(new KeyValuePair<string, object>(SESSION_TOKEN_KEY, sessionToken));
            }
            else
            {
                headers[SESSION_TOKEN_KEY] = sessionToken;
            }
        }

        public string RelativeUrl { get; set; }
        public IDictionary<string, object> Body { get; set; }
        public string Method { get; set; }

        public HttpRequest HttpEncode
        {
            get
            {
                var method = HttpMethod;
                var headers = Headers;
                if (method == "GET" || method == "DELETE")
                {
                    headers = Headers.Filter(new string[] { "Content-Type" });
                }
                return new HttpRequest()
                {
                    Data = Body != null ? new MemoryStream(Encoding.UTF8.GetBytes(Json.Encode(Body))) : null,
                    Headers = headers.ToDictionary(x => x.Key, x => x.Value.ToString()).ToList(),
                    Method = method,
                    Uri = HttpUri
                };
            }
        }

        public IDictionary<string, object> UrlParameters { get; set; }

        private string HttpMethod
        {
            get
            {
                if (Method != null) return Method;
                if (Body != null) return "POST";
                return "GET";
            }
        }

        private Uri HttpUri
        {
            get
            {
                var uriBuilder = new UriBuilder(this.GameRouterSchema, this.GameRouterHost);
                uriBuilder.Path = this.ApiVersion + this.RelativeUrl;

                if (UrlParameters != null)
                {
                    if (UrlParameters.Count > 0)
                    {
                        uriBuilder.Query = AVClient.BuildQueryString(UrlParameters);
                    }
                }
                return uriBuilder.Uri;
            }
        }

        public string SokcetEncode
        {
            get
            {
                this.Body["appId"] = AVClient.CurrentConfiguration.ApplicationId;
                this.Body["peerId"] = Play.peer.ID;
                this.Body["i"] = NextCmdId;

                return Json.Encode(this.Body);
            }
        }
        internal static readonly object Mutex = new object();
        private static Int32 lastCmdId = -65536;
        internal static Int32 NextCmdId
        {
            get
            {
                lock (Mutex)
                {
                    lastCmdId++;

                    if (lastCmdId > ushort.MaxValue)
                    {
                        lastCmdId = -65536;
                    }
                    return lastCmdId;
                }
            }
        }
    }

    public class PlayResponse
    {
        public int StatusCode { get; internal set; }

        public IDictionary<string, object> Body { get; set; }

        public PlayResponse()
        {

        }
        public PlayResponse(IDictionary<string, object> sokcetDataPacket)
        {
            Body = sokcetDataPacket;
            StatusCode = 200;
            if (sokcetDataPacket.ContainsKey("cmd"))
            {
                if (sokcetDataPacket["cmd"].ToString() == "error")
                {
                    StatusCode = int.Parse(sokcetDataPacket["code"].ToString());
                }
            }
        }

        public bool IsSuccessful
        {
            get
            {
                return StatusCode > 199 && StatusCode < 202;
            }
        }

        public string ErrorReason
        {
            get
            {
                return Body.GetValue<string>("reason", null);
            }
        }

        public int ErrorCode
        {
            get
            {
                return Body.GetValue<int>("code", 400);
            }
        }
    }
}
