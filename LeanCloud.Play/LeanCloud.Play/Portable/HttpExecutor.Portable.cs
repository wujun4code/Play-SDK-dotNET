using LeanCloud.Core.Internal;
using LeanCloud.Storage.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LeanCloud
{
    public static class HttpExecutor
    {
        public static void Execute(HttpRequest httpRequest, Action<Tuple<HttpStatusCode, string>> done)
        {
            var task = AVPlugins.Instance.HttpClient.ExecuteAsync(httpRequest, null, null, CancellationToken.None);

            task.Wait();

            done(task.Result);
        }
    }
}
