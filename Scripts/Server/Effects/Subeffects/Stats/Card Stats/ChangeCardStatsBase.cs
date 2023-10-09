using KompasCore.Cards;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Cards;
using Kompas.Effects.Models.Identities.ManyCards;
using Kompas.Gamestate.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public abstract class ChangeCardStatsBase : ServerSubeffect
	{
		public IIdentity<IGameCard> card;
		public IIdentity<IReadOnlyCollection<IGameCard>> cards;

		public IIdentity<int> n;
		public IIdentity<int> e;
		public IIdentity<int> s;
		public IIdentity<int> w;
		public IIdentity<int> c;
		public IIdentity<int> a;

		public IIdentity<int> turnsOnBoard;
		public IIdentity<int> attacksThisTurn;
		public IIdentity<int> spacesMoved;
		public IIdentity<int> duration;

		protected IEnumerable<GameCard> cardsToAffect => cards.From(ResolutionContext, default).Select(c => c.Card);

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);

			card ??= new TargetIndex() { index = targetIndex };
			cards ??= new Concat() { cards = new IIdentity<IGameCard>[] { card } };

			var initContext = DefaultInitializationContext;
			cards.Initialize(initContext);

			n?.Initialize(initContext);
			e?.Initialize(initContext);
			s?.Initialize(initContext);
			w?.Initialize(initContext);
			c?.Initialize(initContext);
			a?.Initialize(initContext);

			turnsOnBoard?.Initialize(initContext);
			attacksThisTurn?.Initialize(initContext);
			spacesMoved?.Initialize(initContext);
			duration?.Initialize(initContext);
		}

		protected void ValidateCardOnBoard(GameCard card)
		{
			if (forbidNotBoard && card.Location != CardLocation.Board)
				throw new InvalidLocationException(card.Location, card, ChangedStatsOfCardOffBoard);
		}
	}
}