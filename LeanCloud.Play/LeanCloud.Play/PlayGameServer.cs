using System;
namespace LeanCloud
{
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

		public Mode ServiceMode { get; set; }

		public Protocol ComunicationProtocol { get; set; }

		/// <summary>
		/// Mode.
		/// </summary>
		public enum Mode
		{
			Public = 0,
			Private = 1,
			SelfHosting = 2
		}

		/// <summary>
		/// Protocol.
		/// </summary>
		public enum Protocol
		{
			WebSokcet = 0,
			TCP = 1,
			UDP = 2
		}

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
