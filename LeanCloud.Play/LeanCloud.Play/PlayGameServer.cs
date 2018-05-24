using System;
namespace LeanCloud
{
    /// <summary>
    /// Play game server.
    /// </summary>
	public class PlayGameServer
	{
		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the secondary URL.
		/// </summary>
		/// <value>The secondary URL.</value>
		public string SecondaryUrl { get; set; }

		/// <summary>
		/// Gets or sets the fetched at.
		/// </summary>
		/// <value>The fetched at.</value>
		public DateTime FetchedAt { get; set; }

		/// <summary>
		/// Gets or sets the ttl.
		/// </summary>
		/// <value>The ttl.</value>
		public int TTL { get; set; }

        /// <summary>
        /// Gets or sets the service mode.
        /// </summary>
        /// <value>The service mode.</value>
		public Mode ServiceMode { get; set; }

        /// <summary>
        /// Gets or sets the comunication protocol.
        /// </summary>
        /// <value>The comunication protocol.</value>
		public Protocol ComunicationProtocol { get; set; }

		/// <summary>
		/// Mode.
		/// </summary>
		public enum Mode
		{
            /// <summary>
            /// The public.
            /// </summary>
			Public = 0,
            /// <summary>
            /// The private.
            /// </summary>
			Private = 1,
            /// <summary>
            /// The self host.
            /// </summary>
			SelfHost = 2
		}

		/// <summary>
		/// Protocol.
		/// </summary>
		public enum Protocol
		{
            /// <summary>
            /// The web sokcet.
            /// </summary>
			WebSokcet = 0,
            /// <summary>
            /// The tcp.
            /// </summary>
			TCP = 1,
            /// <summary>
            /// The UDP.
            /// </summary>
			UDP = 2
		}

        /// <summary>
        /// Fetchs from public cloud.
        /// </summary>
        /// <returns>The from public cloud.</returns>
        /// <param name="response">Response.</param>
		public static PlayGameServer FetchFromPublicCloud(PlayResponse response)
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
