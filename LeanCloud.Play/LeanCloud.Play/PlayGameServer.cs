using System;
namespace LeanCloud
{
    /// <summary>
    /// Play game server.
    /// </summary>
    public class PlayGameServer : PlayServer
    {
        /// <summary>
        /// Fetchs from public cloud.
        /// </summary>
        /// <returns>The from public cloud.</returns>
        /// <param name="response">Response.</param>
        internal static PlayGameServer FetchFromPublicCloud(PlayResponse response)
        {
            if (response.IsSuccessful)
            {
                return new PlayGameServer()
                {
                    FetchedAt = DateTime.Now,
                    Url = response.Body["server"] as string,
                    SecondaryUrl = response.Body["secondary"] as string,
                    TTL = int.Parse(response.Body["ttl"].ToString()),
                    ServiceMode = Mode.Public,
                    ComunicationProtocol = Protocol.WebSokcet
                };
            }
            return null;
        }
    }
}
