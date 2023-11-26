using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Cards.Models;
using Kompas.Client.Networking;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;

namespace Kompas.Client.Gamestate.Search
{
	/// <summary>
	/// Holds the data for a single search
	/// </summary>
	public class CardSearch : ISearch
	{
		public readonly GameCard[] toSearch;
		public readonly IListRestriction listRestriction;
		public readonly IList<GameCard> searched = new List<GameCard>();

		private readonly IGame game;
		protected readonly ClientNotifier clientNotifier;

		/// <summary>
		/// Triggered when the search completes
		/// </summary>
		public event EventHandler? SearchFinished;

		/// <summary>
		/// Whether the list restriction of this search data determines that enough cards have <b>already</b> been searched 
		/// that the search can end before the maximum possible number of cards have been searched.
		/// </summary>
		public bool HaveEnough => listRestriction?.HaveEnough(searched.Count) ?? false;

		/// <summary>
		/// Whether any cards currently able to be searched can't currently be seen and clicked on.
		/// </summary>
		private bool AnyToSearchNotVisible => toSearch.Any(c => c.InHiddenLocation && !c.KnownToEnemy); //TODO confirm this instead of "visible"
		public bool ShouldShowSearchUI => AnyToSearchNotVisible || HaveEnough || listRestriction.GetStashedMaximum() == int.MaxValue;

		public string SearchProgress
		{
			get
			{
				int numSearched = searched.Count;
				int min = listRestriction?.GetStashedMinimum() ?? 0;
				int max = listRestriction?.GetStashedMaximum() ?? 0;

				if (listRestriction == null) return string.Empty;
				else if (min > 0 && max < int.MaxValue)
					return $"{numSearched} / {min} - {max}";
				else if (max < int.MaxValue) return $"{numSearched} / up to {max}";
				else if (min > 0) return $"{numSearched} / at least {min}";
				else return string.Empty;
			}
		}

		protected CardSearch(IEnumerable<GameCard> toSearch, IListRestriction listRestriction,
			IGame game, ClientTargetingController targetingController, ClientNotifier clientNotifier)
		{
			this.toSearch = toSearch.ToArray();
			Array.Sort(this.toSearch);
			this.listRestriction = listRestriction;

			this.game = game;
			this.clientNotifier = clientNotifier;

			foreach (var card in game.Cards) card.CardController.RefreshTargeting();
		}

		/*
		public SearchUIController clientSearchUICtrl;
		public ConfirmTargetsUIController confirmTargetsCtrl; */

		public static CardSearch? StartSearch(IEnumerable<GameCard> toSearch, IListRestriction listRestriction,
			IGame game, ClientTargetingController targetingController, ClientNotifier notifier)
		{
			//if the list is empty, don't search
			if (!toSearch.Any()) return null;

			GD.Print($"Searching a list of {toSearch.Count()} cards: {string.Join(",", toSearch.Select(c => c.CardName))}");
			return new(toSearch, listRestriction, game, targetingController, notifier);
		}

		public void Select(Space space) => GD.Print("Selecting a space while searching for a card does nothing");

		/// <summary>
		/// Adds the target, and sends off the list of targets as necessary 
		/// </summary>
		/// <param name="nextTarget"></param>
		/// <returns></returns>
		public void Select(GameCard nextTarget)
		{
			//if it's already selected, deselect it
			if (searched.Contains(nextTarget)) RemoveTarget(nextTarget);
			//otherwise, deselect
			else AddTarget(nextTarget);
		}

		/// <summary>
		/// Adds the target to the current list of targets, if applicable
		/// </summary>
		/// <param name="nextTarget"></param>
		private void AddTarget(GameCard nextTarget)
		{
			GD.Print($"Tried to add {nextTarget} as next target");

			//check if the target is a valid potential target
			if (!toSearch.Contains(nextTarget))
			{
				GD.PushError($"Tried to target card {nextTarget.CardName} that isn't a valid target");
				return;
			}

			if (listRestriction.Deduplicate(searched).Count()
				== listRestriction.Deduplicate(searched.Append(nextTarget)).Count())
			{
				GD.PushError($"Allowed user to target non-distinct card {nextTarget} when they had already seen {string.Join(",", searched.Select(c => c.CardName))}");
				return;
			}

			searched.Add(nextTarget);
			//TODO make be handled by card view controller
			// Debug.Log($"Added {nextTarget.CardName}, targets are now {string.Join(",", CurrSearchData.Value.searched.Select(c => c.CardName))}");

			if (listRestriction == null) SendTargets();
			//if we were given a maximum number to be searched, and hit that number, no reason to keep asking
			else if (searched.Count == listRestriction.GetStashedMaximum()) SendTargets();

			nextTarget.CardController.RefreshTargeting();
		}

		public void RemoveTarget(GameCard target)
		{
			GD.Print($"Tried to remove {target} as next target");
			searched.Remove(target);
			target.CardController.RefreshTargeting();
		}

		public void SendTargets(bool confirmed = false)
		{
			//TODO load settings
			if (game.Settings?.confirmTargets == Shared.Settings.ConfirmTargets.Prompt && !confirmed)
			{
				//confirmTargetsCtrl.Show(CurrSearchData.searched);
				return;
			}

			SendTargets(searched);
		}

		private void SendTargets(IList<GameCard> choices)
		{
			GD.Print($"Sending targets {string.Join(",", choices.Select(c => c.CardName))} ");

			SendChoices(choices);
			foreach (var card in game.Cards) card.CardController.RefreshTargeting();
			SearchFinished?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void SendChoices(IList<GameCard> choices)
			=> clientNotifier.RequestListChoices(choices);

		public bool IsValidTarget(GameCard card) => toSearch.Contains(card);
		public bool IsCurrentTarget(GameCard card) => searched.Contains(card);
	}
}