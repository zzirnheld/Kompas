using System.Collections.Generic;
using System.Linq;
using Kompas.Effects.Models.Identities.Numbers;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Play
{
	public class AnyOf : AnyOfBase<(Space s, Player p)> { }

	public class StandardPlayRestriction : RestrictionBase<(Space s, Player p)>
	{
		protected override bool IsValidLogic((Space s, Player p) item, IResolutionContext context)
			=> InitializationContext.game.IsValidStandardPlaySpace(item.s, item.p);
	}

	public class PlayRestriction : DualRestrictionBase<(Space s, Player p)>, IPlayRestriction
	{
		[JsonProperty]
		public bool playAsAugment = false;
		[JsonProperty]
		public string[] augmentOnSubtypes;

		[JsonProperty]
		public bool requireStandardAdjacency = true;

		[JsonProperty]
		public IRestriction<(Space s, Player p)>[] recommendations = { };

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			foreach (var r in recommendations) r.Initialize(initializationContext);
		}

		private static IRestriction<(Space s, Player p)> OnOrAdjacentToFriendly() => new AnyOf()
			{
				elements = new IRestriction<(Space s, Player p)>[] {
					new Cards.Friendly(),
					new Spaces.AdjacentTo()
					{
						cardRestriction = new Cards.Friendly()
					}
				}
			};

		protected override IEnumerable<IRestriction<(Space s, Player p)>> DefaultRestrictions
		{
			get
			{
				yield return new Spaces.SpellRule();
				yield return new Gamestate.NoUniqueCopyExists();

				if (playAsAugment)
				{
					if (augmentOnSubtypes != null) yield return new Cards.Subtypes() { subtypes = augmentOnSubtypes };

					//On or adjacent to a friendly
					if (requireStandardAdjacency) yield return OnOrAdjacentToFriendly();
				}
				else
				{
					yield return new Spaces.Empty();

					if (requireStandardAdjacency) yield return new StandardPlayRestriction();
				}
			}
		}

		protected override IEnumerable<IRestriction<(Space s, Player p)>> DefaultNormalRestrictions
		{
			get
			{
				yield return new Gamestate.NothingHappening();

				//Can afford to play
				yield return new Gamestate.NumberFitsRestriction()
				{
					number = new Identities.Numbers.FromCardValue()
					{
						card = new Identities.Cards.ThisCardNow(),
						cardValue = new CardValue() { value = CardValue.Cost }
					},
					restriction = new Restrictions.Numbers.Compare()
					{
						comparison = new Relationships.Numbers.LessThanEqual(),
						other = new Identities.Numbers.Pips() { player = new Identities.Players.FriendlyPlayer() }
					}
				};

				//Currently controls the card in hand
				yield return new Players.Is()
				{
					player = new Identities.Players.ControllerOf() { card = new Identities.Cards.ThisCardNow() }
				};
				yield return new Gamestate.CardFitsRestriction()
				{
					card = new Identities.Cards.ThisCardNow(),
					cardRestriction = new Cards.AtLocation(Location.Hand)
				};
			}
		}

		public bool IsRecommendedNormalPlay((Space s, Player p) item)
			=> IsRecommendedPlay(item, ResolutionContext.PlayerTrigger(null, InitializationContext.game));

		public bool IsRecommendedPlay((Space s, Player p) item, IResolutionContext context)
			=> IsValid(item, context)
			&& recommendations.All(r => r.IsValid(item, context));

		public bool IsValidIgnoringAdjacency((Space s, Player p) item, IResolutionContext context)
			=> IsValidIgnoring(item, context, r => r is StandardPlayRestriction);
	}
}