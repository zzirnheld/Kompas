using System.Net;
using Godot;

namespace Kompas.Client.UI.GameStart
{
	public partial class ConnectToServerController : Control
	{
		[Export]
		private GameStartController? GameStart { get; set; }

		[Export]
		private LineEdit? IP { get; set; }

		public void Connect()
		{
			_ = IP ?? throw new System.NullReferenceException("Failed to init");
			_ = GameStart ?? throw new System.NullReferenceException("Failed to init");
			
			string ip = IP.Text;
			if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";
			else if (!IPAddress.TryParse(ip, out _)) return;

			GameStart.TryConnect(ip);
		}
	}
}