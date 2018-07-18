using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Minimum
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder();
            var connectManager = new ConnectManager("wujun");
            connectManager.Connect("0.0.1");
            connectManager.ConnectedLobby = (c) => 
            {
                c.CreateRoom();
            };
            await hostBuilder.RunConsoleAsync();
        }
    }
}
