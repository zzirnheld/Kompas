using System.Collections.Generic;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
		public class MovementRestriction : DualRestrictionBase<Space>, IMovementRestriction
		{
			public bool moveThroughCards = false; //TODO check this flag when determining how much "movement" the move should cost.
												  //ideally implement some sort of "get move cost to" function here, which can be replaced by an Identity as applicable

			protected override IEnumerable<IRestriction<Space>> DefaultRestrictions
			{
				get
				{
					//Card must be in play
					yield return new Gamestate.CardFitsRestriction()
					{
						card = new Identities.Cards.ThisCardNow(),
						cardRestriction = new Cards.AtLocation(Location.Board)
					};

					//TODO re-add swapping probably. will req DefaultEffectElements
					yield return new Empty();

					//Can't "move" to the space the card is in now
					yield return new Different()
					{
						from = new Identities.Cards.ThisCardNow()
					};

					yield return new SpellRule();
				}
			}

			/// <summary>
			/// Restrictions that, by default, apply to a player moving a card normally (but not by effect)
			/// </summary>
			protected override IEnumerable<IRestriction<Space>> DefaultNormalRestrictions
			{
				get
				{
					//Only characters can move, normally
					yield return new Gamestate.CardFitsRestriction()
					{
						card = new Identities.Cards.ThisCardNow(),
						cardRestriction = new Cards.Character()
					};
					yield return new CompareDistance()
					{
						//If you can move through cards, you just care about the taxicab distance.
						//Most cards have to move through an empty path
						shortestEmptyPath = !moveThroughCards,
						distanceTo = new Identities.Cards.ThisCardNow(),
						comparison = new Relationships.Numbers.LessThanEqual(),
						number = new Identities.Numbers.FromCardValue()
						{
							cardValue = new Identities.Numbers.CardValue() { value = Identities.Numbers.CardValue.SpacesCanMove },
							card = new Identities.Cards.ThisCardNow()
						}
					};
					yield return new Gamestate.NothingHappening();
					yield return new Gamestate.FriendlyTurn();
				}
			}

			public bool WouldBeValidNormalMoveInOpenGamestate(Space item)
				=> NormalRestriction.IsValidIgnoring(item, ResolutionContext.PlayerTrigger(null, InitializationContext.game),
					restriction => restriction is Gamestate.NothingHappening); //ignore req that nothing is happening
		}
}