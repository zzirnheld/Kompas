using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Models.Restrictions.Cards;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	public abstract class HandSizeStackable : IStackable
	{
		public GameCard? Card => null;

		protected readonly IGame game;
		protected readonly IPlayer player;

		private IRestriction<IGameCardInfo>? handSizeCardRestriction;
		public IRestriction<IGameCardInfo> HandSizeCardRestriction => handSizeCardRestriction ??= CreateHandSizeCardRestriction();

		public IPlayer? ControllingPlayer => player;

		protected HandSizeStackable(IGame game, IPlayer player)
		{
			this.game = game;
			this.player = player;
		}

		private IRestriction<IGameCardInfo> CreateHandSizeCardRestriction()
		{
			var ret = new AllOf()
			{
				elements = new IRestriction<IGameCardInfo>[]
				{
					new Friendly(),
					new AtLocation() { locations = new string[]{ "Hand" } }
				}
			};
			ret.Initialize(new InitializationContext(game: game, source: default, controller: player));
			return ret;
		}

		public GameCard? GetCause(IGameCardInfo? withRespectTo) => Card;
	}
}