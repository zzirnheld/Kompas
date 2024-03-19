using Godot;
using Kompas.Cards.Controllers;
using Kompas.Gamestate;
using Kompas.Godot;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class PlaceableSpaceController : Node3D
	{
		[Export]
		private int x;
		[Export]
		private int y;

		public Space Space => (x, y);

		public void Place (ICardController card)
		{
			this.TransferChild(card.Node);
			card.Node.Visible = true;
			card.Node.Scale = Vector3.One;
			card.Node.Position = Vector3.Zero;
		}
	}
}