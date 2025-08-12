using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Cards.Models;
using Kompas.Client.Networking;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;

namespace Kompas.Client.Gamestate.Search;

/// <summary>
/// Holds the data for a single search
/// </summary>
public class CardSearch : ISearch
{
	public readonly GameCard[] validTargets;
	public readonly IListRestriction listRestriction;
	public readonly ISet<int> toSearchIDs;
	public readonly IList<GameCard> currentTargets = new List<GameCard>();

	private readonly IGame game;
	protected readonly ClientNotifier clientNotifier;

	/// <summary>
	/// Triggered when the search completes
	/// </summary>
	public event EventHandler? SearchFinished;

	/// <summary>
	/// Triggered when the following changes: <br/>
	/// Whether the search has enough cards that the player may decline further targets,
	/// but not so many that we hit the maximum.
	/// </summary>
	public event EventHandler<bool>? HaveEnoughChanged;

	public bool HaveEnough => currentTargets.Count >= listRestriction.GetStashedMinimum();

	public IReadOnlyCollection<(Location, bool)> SearchedLocations { get; }

	public string SearchProgress
	{
		get
		{
			int numSearched = currentTargets.Count;
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

	protected CardSearch(IEnumerable<GameCard> validTargets, IListRestriction listRestriction, IEnumerable<int> toSearchIDs,
		IGame game, ClientNotifier clientNotifier)
	{
		this.validTargets = validTargets.ToArray();
		this.listRestriction = listRestriction;
		this.toSearchIDs = new HashSet<int>(toSearchIDs);

		this.game = game;
		this.clientNotifier = clientNotifier;

		SearchedLocations = validTargets.Select(c => (c.Location, c.ControllingPlayer.Friendly)).Distinct().ToArray();
	}

	public static CardSearch? Create(IEnumerable<GameCard> validTargets, IListRestriction listRestriction, IEnumerable<int> toSearchIDs,
		IGame game, ClientNotifier notifier)
	{
		//if the list is empty, don't search
		if (!validTargets.Any()) return null;

		Logger.Log($"Searching a list of {validTargets.Count()} cards: {string.Join(",", validTargets.Select(c => c.CardName))}, from {string.Join(",", toSearchIDs)}."
			+ $"Min of {listRestriction.GetStashedMinimum()}, max of {listRestriction.GetStashedMaximum()}");
		return new(validTargets, listRestriction, toSearchIDs, game, notifier);
	}

	public void Select(Space space) => Logger.Log("Selecting a space while searching for a card does nothing");

	/// <summary>
	/// Adds the target, and sends off the list of targets as necessary 
	/// </summary>
	/// <param name="nextTarget"></param>
	/// <returns></returns>
	public void Select(GameCard nextTarget)
	{
		//if it's already selected, deselect it
		if (currentTargets.Contains(nextTarget)) RemoveTarget(nextTarget);
		//otherwise, deselect
		else AddTarget(nextTarget);
	}

	/// <summary>
	/// Adds the target to the current list of targets, if applicable
	/// </summary>
	/// <param name="nextTarget"></param>
	private void AddTarget(GameCard nextTarget)
	{
		Logger.Log($"Tried to add {nextTarget} as next target");
		var hadEnough = HaveEnough;

		//check if the target is a valid potential target
		if (!validTargets.Contains(nextTarget))
		{
			Logger.Err($"Tried to target card {nextTarget.CardName} that isn't a valid target");
			return;
		}

		if (listRestriction.Deduplicate(currentTargets).Count()
			== listRestriction.Deduplicate(currentTargets.Append(nextTarget)).Count())
		{
			Logger.Err($"Allowed user to target non-distinct card {nextTarget} when they had already seen {string.Join(",", currentTargets.Select(c => c.CardName))}");
			return;
		}

		currentTargets.Add(nextTarget);
		//TODO make be handled by card view controller
		// Debug.Log($"Added {nextTarget.CardName}, targets are now {string.Join(",", CurrSearchData.Value.searched.Select(c => c.CardName))}");

		if (listRestriction == null) SendTargets();
		//if we were given a maximum number to be searched, and hit that number, no reason to keep asking
		else if (currentTargets.Count == listRestriction.GetStashedMaximum()) SendTargets();
		else if (!hadEnough && HaveEnough) HaveEnoughChanged?.Invoke(this, true);

		nextTarget.CardController.RefreshTargeting();
	}

	public void RemoveTarget(GameCard target)
	{
		var hadEnough = HaveEnough;
		Logger.Log($"Tried to remove {target} as next target");
		currentTargets.Remove(target);
		target.CardController.RefreshTargeting();

		if (hadEnough && !HaveEnough) HaveEnoughChanged?.Invoke(this, false);
	}

	public bool SendIfHaveEnough()
	{
		if (HaveEnough)
		{
			SendTargets();
			return true;
		}

		return false;
	}

	public void SendTargets(bool confirmed = false)
	{
		//TODO load settings
		if (game.Settings?.confirmTargets == Shared.Settings.ConfirmTargets.Prompt && !confirmed)
		{
			//confirmTargetsCtrl.Show(CurrSearchData.searched);
			return;
		}

		SendTargets(currentTargets);
	}

	private void SendTargets(IList<GameCard> targets)
	{
		Logger.Log($"Sending targets {string.Join(",", targets.Select(c => c.CardName))} ");

		SendChoices(targets);
		foreach (var card in game.Cards) card.CardController.RefreshTargeting();
		SearchFinished?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void SendChoices(IList<GameCard> choices)
		=> clientNotifier.RequestListChoices(choices);

	public bool IsValidTarget(GameCard card) => validTargets.Contains(card);
	public bool IsCurrentTarget(GameCard card) => currentTargets.Contains(card);
	public bool IsBeingSearched(GameCard card) => toSearchIDs.Contains(card.ID);

	public bool IsRecommendedTarget(Space space) => false;
	public bool IsUnrecommendedTarget(Space space) => false;
	public bool IsCurrentTarget(Space space) => false;
}