using Kompas.Cards.Models;
using Kompas.Gamestate.Locations.Models;
using Kompas.Networking;

namespace Kompas.Gamestate.Players
{
	public interface IPlayer
	{
		public int HandSizeLimit => 7;

		public IGame Game { get; }
		public Networker Networker { get; }

		public IPlayer Enemy { get; }

		//game mechanics data
		public int Pips { get; set; }
		/// <summary>
		/// Just to let the player know. Should not be referenced, unless I want to add "gains more pips next turn" effects that aren't delayed for some reason
		/// </summary>
		public int PipsNextTurn { set; }
		public IGameCard Avatar { get; }

		//other game data
		/// <summary>
		/// Whether the player represented by this IPlayer is the POV player.
		/// </summary>
		public bool Friendly { get; }
		public int Index { get; }

		public bool HandFull => Hand.HandSize >= HandSizeLimit;
		public Space AvatarCorner => Index == 0 ? Space.NearCorner : Space.FarCorner;

		//friendly
		public IDeck Deck { get; }
		public IDiscard Discard { get; }
		public IHand Hand { get; }
		public IAnnihilation Annihilation { get; }

		public Space SubjectiveCoords(Space space) => Index == 0 ? space : space.Inverse;

		public PlayerController PlayerController { get; }
	}

	public interface IPlayer<CardType, PlayerType> : IPlayer
		where CardType : class, IGameCard<CardType, PlayerType>
		where PlayerType : IPlayer<CardType, PlayerType>
	{
		public new PlayerType Enemy { get; }

		public new IDeck<CardType, PlayerType> Deck { get; }
		public new IDiscard<CardType, PlayerType> Discard { get; }
		public new IHand<CardType, PlayerType> Hand { get; }
		public new IAnnihilation<CardType, PlayerType> Annihilation { get; }
	}
}