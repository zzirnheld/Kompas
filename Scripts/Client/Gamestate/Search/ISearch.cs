using System;
using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;

namespace Kompas.Client.Gamestate.Search;

public interface ISearch
{
	public event EventHandler? SearchFinished;

	public IReadOnlyCollection<(Location location, bool friendly)> SearchedLocations { get; }

	public void Select(GameCard card);
	public void Select(Space space);

	public bool IsValidTarget(GameCard card);
	public bool IsCurrentTarget(GameCard card);
	public bool IsBeingSearched(GameCard card);

	/// <summary>
	/// Is a valid target, and recommended.
	/// (ex: playing a spell that requires an adjacent character, there's an adjacent character)
	///</summary>
	public bool IsRecommendedTarget(Space space);
	/// <summary>
	/// Is a valid target, but not recommended.
	/// (ex: playing a spell that requires an adjacent character, there's NOT an adjacent character)
	///</summary>
	public bool IsUnrecommendedTarget(Space space);
	public bool IsCurrentTarget(Space space);

	/// <summary>
	/// If the search has sufficient targets,
	/// sends that list of targets.
	/// This will only happen if the minimum number of targets
	/// is strictly less than the maximum,
	/// and the player has already selected enough targets.
	/// </summary>
	/// <returns>
	/// <see langword="true"/> if the search had a minimum,
	/// and had enough cards to send the given choices.
	/// <see langword="false"/> otherwise.
	/// </returns>
	public bool SendIfHaveEnough();
}