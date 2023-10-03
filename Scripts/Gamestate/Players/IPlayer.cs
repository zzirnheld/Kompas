using Kompas.Cards.Models;
using Kompas.Gamestate.Locations.Models;

namespace Kompas.Gamestate.Players
{
	public interface IPlayer
	{
		public int HandSizeLimit => 7;

		public Game Game { get; }

		public IPlayer Enemy { get; }

		//game mechanics data
		public int Pips { get; set; }
		public GameCard Avatar { get; set; }

		//other game data
		public bool Friendly { get; }
		public int Index { get; }

		public bool HandFull => Hand.HandSize >= HandSizeLimit;
		public Space AvatarCorner => Index == 0 ? Space.NearCorner : Space.FarCorner;

		//friendly
		public abstract Deck Deck { get; }
		public abstract Discard Discard { get; }
		public abstract Hand Hand { get; }
		public abstract Annihilation Annihilation { get; }

		public Space SubjectiveCoords(Space space) => Index == 0 ? space : space.Inverse;
	}
}