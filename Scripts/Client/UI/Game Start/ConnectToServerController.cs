using Godot;

namespace Kompas.Client.UI.GameStart
{
	public partial class ConnectToServerController : Control
	{
		[Export]
		private GameStartController GameStart { get; set; }

		[Export]
		private LineEdit IP { get; set; }

		public void Connect()
		{
			string ip = IP.Text;
			if (ip == string.Empty) ip = "127.0.0.1";
			GameStart.TryConnect(ip);
		}
	}
}