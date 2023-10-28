using Godot;
using Kompas.Cards.Models;
using Kompas.Client.UI;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Players
{
	public partial class ClientPlayerController : PlayerController
	{
		[Export]
		private PlayerInfoController PlayerInfo { get; set; }

		public override IGameCard Avatar { set => PlayerInfo.AvatarTexture = value.CardFaceImage; }
		public override int Pips { set => PlayerInfo.PipsCount = value; }
		public override int PipsNextTurn { set => PlayerInfo.NextTurnPipsCount = value; }
	}
}