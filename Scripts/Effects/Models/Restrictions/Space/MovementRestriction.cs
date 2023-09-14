using System.Collections.Generic;
using Kompas.Gamestate;

namespace Kompas.Effects.Models.Restrictions.SpaceRestrictions
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
					yield return new TriggerRestrictionElements.CardFitsRestriction()
					{
						card = new Identities.Cards.ThisCardNow(),
						cardRestriction = new CardRestrictionElements.Location(Location.Board)
					};

					//TODO re-add swapping probably. will req DefaultEffectElements
					yield return new SpaceRestrictionElements.Empty();

					//Can't "move" to the space the card is in now
					yield return new SpaceRestrictionElements.Different()
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
					yield return new TriggerRestrictionElements.CardFitsRestriction()
					{
						card = new Identities.Cards.ThisCardNow(),
						cardRestriction = new CardRestrictionElements.Character()
					};
					yield return new SpaceRestrictionElements.CompareDistance()
					{
						//If you can move through cards, you just care about the taxicab distance.
						//Most cards have to move through an empty path
						shortestEmptyPath = !moveThroughCards,
						distanceTo = new Identities.Cards.ThisCardNow(),
						comparison = new Relationships.NumberRelationships.LessThanEqual(),
						number = new Identities.Numbers.FromCardValue()
						{
							cardValue = new CardValue() { value = CardValue.SpacesCanMove },
							card = new Identities.Cards.ThisCardNow()
						}
					};
					yield return new GamestateRestrictionElements.NothingHappening();
					yield return new GamestateRestrictionElements.FriendlyTurn();
				}
			}

			public bool WouldBeValidNormalMoveInOpenGamestate(Space item)
				=> NormalRestriction.IsValidIgnoring(item, ResolutionContext.PlayerTrigger(null, InitializationContext.game),
					restriction => restriction is GamestateRestrictionElements.NothingHappening); //ignore req that nothing is happening
		}
}