using LeanCloud.Storage.Internal;
using System;
using System.IO;
using System.Net;
using UnityEngine.Networking;

namespace LeanCloud
{
    public static class HttpExecutor
    {
        public static void Execute(HttpRequest httpRequest, Action<Tuple<HttpStatusCode, string>> done)
        {
            IDisposable toDisposeAfterReading = null;
            byte[] bytes = null;
            if (httpRequest.Data != null)
            {
                var ms = new MemoryStream();
                toDisposeAfterReading = ms;
                httpRequest.Data.CopyTo(ms);
                bytes = ms.ToArray();
            }

            Dispatcher.Instance.Post(() =>
            {
                WaitForWebRequest(GenerateRequest(httpRequest, bytes), request =>
                {
                    if (request.isDone)
                    {
                        var statusCode = GetResponseStatusCode(request);
                        if (!String.IsNullOrEmpty(request.error) && String.IsNullOrEmpty(request.downloadHandler.text))
                        {
                            var errorString = string.Format("{{\"error\":\"{0}\"}}", request.error);
                            done(new Tuple<HttpStatusCode, string>(statusCode, errorString));
                        }
                        else
                        {
                            done(new Tuple<HttpStatusCode, string>(statusCode, request.downloadHandler.text));
                        }
                    }
                });
            });
        }

        private static HttpStatusCode GetResponseStatusCode(UnityWebRequest request)
        {
            if (Enum.IsDefined(typeof(HttpStatusCode), (int)request.responseCode))
            {
                return (HttpStatusCode)request.responseCode;
            }
            return (HttpStatusCode)400;
        }

        private static UnityWebRequest GenerateRequest(HttpRequest request, byte[] bytes)
        {
            var webRequest = new UnityWebRequest();
            webRequest.method = request.Method;
            webRequest.url = request.Uri.AbsoluteUri;
            // Explicitly assume a JSON content.
            webRequest.SetRequestHeader("Content-Type", "application/json");
            //webRequest.SetRequestHeader("User-Agent", "net-unity-" + AVVersionInfo.Version);
            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    webRequest.SetRequestHeader(header.Key as string, header.Value as string);
                }
            }

            if (bytes != null)
            {
                webRequest.uploadHandler = new UploadHandlerRaw(bytes);
            }
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.Send();
            return webRequest;
        }
        private static void WaitForWebRequest(UnityWebRequest request, Action<UnityWebRequest> action)
        {
            Dispatcher.Instance.Post(() =>
            {
                var isDone = request.isDone;
                action(request);
                if (!isDone)
                {
                    WaitForWebRequest(request, action);
                }
            });
        }
    }
}
