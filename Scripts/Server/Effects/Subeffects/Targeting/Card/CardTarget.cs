using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.ManyCards;
using Kompas.Effects.Models.Restrictions;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions.Gamestate;
using Kompas.Gamestate.Locations;
using Kompas.Server.Networking;
using Newtonsoft.Json;
using System;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class CardTarget : ServerSubeffect
	{
		[JsonProperty]
		public string blurb = string.Empty;
		[JsonProperty]
		public bool secretTarget = false;

		[JsonProperty]
		public IIdentity<IReadOnlyCollection<IGameCardInfo>> toSearch = new All();

		/// <summary>
		/// Restriction that each card must fulfill
		/// </summary>
		[JsonProperty]
		public IRestriction<IGameCardInfo> cardRestriction = new AlwaysValid();

		/// <summary>
		/// Restriction that the list collectively must fulfill
		/// </summary>
		[JsonProperty]
		public IListRestriction listRestriction = IListRestriction.SingleElement;

		/// <summary>
		/// Identifies a card that this target should be linked with.
		/// Usually null, but if you plan on having a delay later, probably a good idea
		/// </summary>
		[JsonProperty]
		public IIdentity<IGameCardInfo>? toLinkWith;
		[JsonProperty]
		public Color linkColor = CardLink.DefaultColor; // "r": #, "g" ... etc

		protected IReadOnlyCollection<GameCard>? stashedPotentialTargets;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			
			toSearch.Initialize(DefaultInitializationContext);
			cardRestriction.Initialize(DefaultInitializationContext);
			listRestriction.Initialize(DefaultInitializationContext);

			toLinkWith?.Initialize(DefaultInitializationContext);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction.AdjustSubeffectIndices(increment, startingAtIndex);
			listRestriction.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		protected IReadOnlyCollection<GameCard> DeterminePossibleTargets()
		{
			var possibleTargets = from card in toSearch.From(ResolutionContext, ResolutionContext)
									where cardRestriction.IsValid(card, ResolutionContext)
									select card.Card;
			return possibleTargets.ToArray();
		}

		protected virtual Task<ResolutionInfo> NoPossibleTargets()
			=> Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

		public override bool IsImpossible (TargetingContext? overrideContext = null)
			=> !listRestriction.AllowsValidChoice(DeterminePossibleTargets(), ResolutionContext);

		public override async Task<ResolutionInfo> Resolve()
		{
			stashedPotentialTargets = DeterminePossibleTargets();
			//if there's no possible valid combo, throw effect impossible
			if (!listRestriction.AllowsValidChoice(stashedPotentialTargets, ResolutionContext))
			{
				Logger.Log($"List restriction {listRestriction} finds no possible list of targets among potential targets" +
					$"{string.Join(",", stashedPotentialTargets.Select(c => c.CardName))}");
				return await NoPossibleTargets();
			}

			//If there's no potential targets, but no targets is a valid choice, then just go to the next effect
			if (!stashedPotentialTargets.Any())
			{
				Logger.Log("An empty list of targets was a valid choice, but there's no targets that can be chosen. Skipping to next effect...");
				return ResolutionInfo.Next;
			}
			else if (listRestriction.GetMaximum(ResolutionContext) == 0)
			{
				Logger.Log("An empty list of targets was a valid choice, and the max to be chosen was 0. Skipping to next effect...");
				return ResolutionInfo.Next;
			}

			IEnumerable<GameCard>? targets = null;
			do {
				targets = await RequestTargets();
				if (targets == null && ServerEffect.CanDeclineTarget) return ResolutionInfo.Impossible(DeclinedFurtherTargets);
			} while (!AddListIfLegal(targets));

			return ResolutionInfo.Next;
		}

		protected async Task<IEnumerable<GameCard>?> RequestTargets()
		{
			string name = Effect.Card.CardName;
			_ = stashedPotentialTargets ?? throw new InvalidOperationException("Tried to add list of targets before asking for targets!");
			int[] targetIds = stashedPotentialTargets.Select(c => c.ID).ToArray();
			Logger.Log($"Potential targets {string.Join(", ", targetIds)}");
			listRestriction.PrepareForSending(ResolutionContext);

			var player = PlayerTarget ?? throw new InvalidOperationException("Tried to send targets to noone!");
			return await ServerGame.Awaiter.GetCardListTargets(player, name, blurb, targetIds, listRestriction);
		}

		public bool AddListIfLegal(IEnumerable<GameCard>? choices)
		{
			Logger.Log($"Potentially adding list {string.Join(",", choices ?? new List<GameCard>())}");
			if (choices == null) return false;

			_ = stashedPotentialTargets ?? throw new InvalidOperationException("Tried to add list of targets before asking for targets!");
			if (choices.Except(stashedPotentialTargets).Any()) return false; //Tried to choose cards that weren't allowed
			if (!listRestriction.IsValid(choices, ResolutionContext)) return false;
			ShuffleIfAppropriate(stashedPotentialTargets);

			//add all cards in the chosen list to targets
			AddList(choices);
			ServerNotifier.AcceptTarget(PlayerTarget ?? throw new InvalidOperationException("Accepted no one's target!?"));
			return true;
		}

		private static void ShuffleIfAppropriate(IEnumerable<GameCard> potentialTargets)
		{
			//TODO replace with polymorphic "shuffle if appropriate" method
			var decksViewed = potentialTargets.Where(c => c.Location == Location.Deck)
							.GroupBy(c => c.LocationModel)
							.Select(grouping => grouping.Key)
							.Cast<Kompas.Gamestate.Locations.Models.Deck>(); //If this cast fails, we have a non-deck controller trying to act like one. If you do this, make it an interface
			foreach (var deck in decksViewed) deck.Shuffle();
		}

		protected virtual void AddList(IEnumerable<GameCard> choices)
		{
			var cardToLinkWith = toLinkWith?.From(ResolutionContext, ResolutionContext)?.Card;
			foreach (var c in choices)
			{
				ServerEffect.AddTarget(c, secretTarget ? PlayerTarget : null);
				if (cardToLinkWith != null) ServerEffect.CreateCardLink(linkColor, secretTarget ? PlayerTarget : null, c, cardToLinkWith);
			}
		}
	}
}