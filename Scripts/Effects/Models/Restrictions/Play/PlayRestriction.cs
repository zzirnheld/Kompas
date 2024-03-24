using System;
using System.Collections.Generic;
using System.Linq;
using Kompas.Effects.Models.Identities.Numbers;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Play
{
	public class AnyOf : AnyOfBase<(Space? s, IPlayer? p)> { }

	public class StandardPlayRestriction : RestrictionBase<(Space? s, IPlayer? p)>
	{
		protected override bool IsValidLogic((Space? s, IPlayer? p) item, IResolutionContext context)
			=> InitializationContext.game.IsValidStandardPlaySpace(item.s, item.p);
	}

	public class PlayRestriction : DualRestrictionBase<(Space? s, IPlayer? p)>, IPlayRestriction
	{
		[JsonProperty]
		public bool playAsAugment = false;
		#nullable disable
		[JsonProperty]
		public string[] augmentOnSubtypes;
		#nullable restore

		[JsonProperty]
		public bool requireStandardAdjacency = true;

		[JsonProperty]
		public IRestriction<(Space? s, IPlayer? p)>[] recommendations = System.Array.Empty<IRestriction<(Space? s, IPlayer? p)>>();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			foreach (var r in recommendations) r.Initialize(initializationContext);
		}

		private static IRestriction<(Space? s, IPlayer? p)> OnOrAdjacentToFriendly() => new AnyOf()
			{
				elements = new IRestriction<(Space? s, IPlayer? p)>[] {
					new Cards.Friendly(),
					new Spaces.AdjacentTo()
					{
						cardRestriction = new Cards.Friendly()
					}
				}
			};

		protected override IEnumerable<IRestriction<(Space? s, IPlayer? p)>> DefaultRestrictions
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

		protected override IEnumerable<IRestriction<(Space? s, IPlayer? p)>> DefaultNormalRestrictions
		{
			get
			{
				yield return new Gamestate.NothingHappening();

				//Can afford to play
				yield return new Gamestate.NumberFitsRestriction()
				{
					number = new FromCardValue()
					{
						card = new Identities.Cards.ThisCardNow(),
						cardValue = new CardValue() { value = CardValue.Cost }
					},
					restriction = new Numbers.Compare()
					{
						comparison = new Relationships.Numbers.LessThanEqual(),
						other = new Pips() { player = new Identities.Players.FriendlyPlayer() }
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

		public bool IsRecommendedNormalPlay((Space? s, IPlayer? p) item)
			=> IsRecommendedPlay(item, IResolutionContext.PlayerAction(item.p ?? throw new NullReferenceException("No player to play!")));

		public bool IsRecommendedPlay((Space? s, IPlayer? p) item, IResolutionContext context)
			=> IsValid(item, context)
			&& recommendations.All(r => r.IsValid(item, context));

		public bool IsValidIgnoringAdjacency((Space? s, IPlayer? p) item, IResolutionContext context)
			=> IsValidIgnoring(item, context, r => r is StandardPlayRestriction);
	}
}