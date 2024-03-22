using System.Collections.Generic;
using Godot;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;

namespace Kompas.Cards.Models
{
	/// <summary>
	/// Something that has all the same information as a card in a game,
	/// but isn't necessarily the actual card
	/// </summary>
	public interface IGameCardInfo
	{
		public IGameCard Card { get; }
		public IGame Game { get; }

		public string CardName { get; }
		public string FileName { get; }

		public char Type { get; }

		public int IndexInList { get; }
		public Location Location { get; }
		public Space? Position { get; }

		public string EffText { get; }
		public string SubtypeText { get; }

		public string[] SpellSubtypes { get; }
		public int Radius { get; }
		public int Duration { get; }

		public bool Activated { get; }
		public bool Negated { get; }
		public bool Unique { get; }
		public bool KnownToEnemy { get; }
		public bool IsAvatar { get; }
		public bool Summoned { get; }

		public int N { get; }
		public int E { get; }
		public int S { get; }
		public int W { get; }
		public int C { get; }
		public int A { get; }

		public int Cost { get; } //TODO move to extensions? no, should be a property here
		
		public int BaseN { get; }
		public int BaseE { get; }
		public int BaseS { get; }
		public int BaseW { get; }
		public int BaseC { get; }
		public int BaseA { get; }
		public bool Hurt => Type == 'C' && Location == Location.Board && E < BaseE;

		public IEnumerable<IGameCard> Augments { get; }
		public IGameCard? AugmentedCard { get; }
		public int SpacesCanMove { get; }

		public IPlayRestriction PlayRestriction { get; }
		public IMovementRestriction MovementRestriction { get; }
		public IRestriction<IGameCardInfo> AttackingDefenderRestriction { get; }

		public IPlayer ControllingPlayer { get; }
		public Space? SubjectivePosition
			=> Position == null ? null : ControllingPlayer.SubjectiveCoords(Position);

		public Texture2D? CardFaceImage { get; }
	}
}