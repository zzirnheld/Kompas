using Kompas.Gamestate;

namespace Kompas.Client.Gamestate
{
	public partial class ClientGameController : GameController
	{

		private ClientGame game;

		public override void _Ready()
		{
			base._Ready();
			game = ClientGame.Create(this);
		}
	}
}