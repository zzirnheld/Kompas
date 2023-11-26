using Kompas.Effects.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.ManyCards;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Models.Identities.Numbers;
using Kompas.Effects.Models.Restrictions.Gamestate;
using Newtonsoft.Json;
using System;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class AutoTarget : ServerSubeffect
	{
		public const string Maximum = "Maximum";
		public const string Any = "Any";
		public const string RandomCard = "Random";

		[JsonProperty]
		public IIdentity<IReadOnlyCollection<IGameCardInfo>> toSearch = new All();
		[JsonProperty]
		public IRestriction<IGameCardInfo> cardRestriction = new AlwaysValid();
		[JsonProperty]
		public CardValue? tiebreakerValue;
		[JsonProperty]
		public string tiebreakerDirection = string.Empty;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			toSearch.Initialize(DefaultInitializationContext);
			cardRestriction.Initialize(DefaultInitializationContext);
			tiebreakerValue?.Initialize(DefaultInitializationContext);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction?.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		public override bool IsImpossible (TargetingContext? overrideContext = null)
			=> !Game.Cards.Any(c => cardRestriction.IsValid(c, ResolutionContext));

		private static GameCard GetRandomCard(GameCard[] cards)
		{
			var random = new System.Random();
			return cards[random.Next(cards.Length)];
		}

		public override Task<ResolutionInfo> Resolve()
		{
			GameCard? potentialTarget = null;
			IEnumerable<GameCard>? potentialTargets = null;
			try
			{
                var gameCardInfos = toSearch.From(ResolutionContext, ResolutionContext)
					?? throw new InvalidOperationException();
                potentialTargets = gameCardInfos
                    .Where(c => cardRestriction.IsValid(c, ResolutionContext))
					.Select(c => c.Card);
				potentialTarget = tiebreakerDirection switch
				{
					Maximum => GetMaximum(potentialTargets),
					Any => potentialTargets.First(),
					RandomCard => GetRandomCard(potentialTargets.ToArray()),
					_ => potentialTargets.Single(),
				};
			}
			catch (System.InvalidOperationException)
			{
				GD.PrintErr($"More than one card fit the card restriction {cardRestriction} " +
					$"for the effect {Effect.blurb} of {Effect.Card.CardName}. Those cards were {potentialTargets}");
				return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
			}

			ServerEffect.AddTarget(potentialTarget);
			return Task.FromResult(ResolutionInfo.Next);
		}

		private GameCard GetMaximum(IEnumerable<GameCard> potentialTargets)
		{
			_ = tiebreakerValue ?? throw new InvalidOperationException();
			return potentialTargets
				.OrderByDescending(tiebreakerValue.GetValueOf)
				.First();
		}
	}
}