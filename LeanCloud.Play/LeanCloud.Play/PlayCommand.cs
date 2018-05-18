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
		internal readonly string NorthChinaGameRouterHost = "game-router-cn-n1.leancloud.cn";
		internal readonly string EastChinaGameRouterHost = "game-router-cn-e1.leancloud.cn";


		internal string GameRouterHost
		{
			get
			{
				if (AVClient.CurrentConfiguration.Region == AVClient.Configuration.AVRegion.Vendor_Tencent)
				{
					return EastChinaGameRouterHost;
				}
				return NorthChinaGameRouterHost;
			}
		}

		internal readonly string ApiVersion = "v1";
		internal static IDictionary<string, object> headers;
        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>The headers.</value>
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

        /// <summary>
        /// Gets or sets the relative URL.
        /// </summary>
        /// <value>The relative URL.</value>
		public string RelativeUrl { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
		public IDictionary<string, object> Body { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
		public string Method { get; set; }

        /// <summary>
        /// Gets the http encode.
        /// </summary>
        /// <value>The http encode.</value>
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

        /// <summary>
        /// Gets or sets the URL parameters.
        /// </summary>
        /// <value>The URL parameters.</value>
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

        /// <summary>
        /// Gets the sokcet encode.
        /// </summary>
        /// <value>The sokcet encode.</value>
		public string SokcetEncode
		{
			get
			{
				this.Body["appId"] = AVClient.CurrentConfiguration.ApplicationId;
				this.Body["peerId"] = Play.peer.ID;
				SocketCommandId = NextCmdId;
				this.Body["i"] = SocketCommandId;

				return Json.Encode(this.Body);
			}
		}

		internal int SocketCommandId
		{
			get; set;
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

    /// <summary>
    /// Play response.
    /// </summary>
	public class PlayResponse
	{
		/// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>The status code.</value>
		public int StatusCode { get; internal set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
		public IDictionary<string, object> Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LeanCloud.PlayResponse"/> class.
        /// </summary>
		public PlayResponse()
		{

		}
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LeanCloud.PlayResponse"/> class.
        /// </summary>
        /// <param name="sokcetDataPacket">Sokcet data packet.</param>
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

        /// <summary>
        /// Gets the error reason.
        /// </summary>
        /// <value>The error reason.</value>
		public string ErrorReason
		{
			get
			{
				return Body.GetValue<string>("reason", null);
			}
		}

        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>The error code.</value>
		public int ErrorCode
		{
			get
			{
				return Body.GetValue<int>("code", 400);
			}
		}
	}
}
