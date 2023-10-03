using Kompas.Client.Gamestate.Locations.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Players
{
	public class ClientPlayer : Player
	{
		private readonly ClientPlayer enemy;
		public override Player Enemy => enemy;

		private readonly ClientGame game;
		public override Game Game => game;

		//TODO pips. does it even need a model? i guess it's insurance in case i add "gain some pips next turn only" but i'd just do that as a delayed...

		private readonly ClientDeck deck;
		public override Deck Deck => deck;
		
		private readonly ClientHand hand;
		public override Hand Hand => hand;

		private readonly ClientDiscard discard;
		public override Discard Discard => discard;

		private readonly ClientAnnihilation annihilation;
		public override Annihilation Annihilation => annihilation;

		public override bool Friendly => index == 0;

		public override int Pips
		{
			get => base.Pips;
			set
			{
				base.Pips = value;
			}
		}
	}
}