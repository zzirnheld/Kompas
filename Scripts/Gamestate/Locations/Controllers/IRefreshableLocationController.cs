using Kompas.Cards.Controllers;

namespace Kompas.Gamestate.Locations.Controllers;

/// <summary>
/// Interface describing a controller for a location that can be refreshed.
/// That is, a location that, when updates happen to its contents,
/// needs to be fully refreshed (as opposed to the <see cref="BoardController"/>,
/// which needs to definitely *not* do that )
/// </summary>
public interface IRefreshableLocationController
{
	/// <summary>
	/// Updates what's shown for this location,
	/// but can be a no-op if the location is not currently being shown
	/// (to not waste processing)
	/// </summary>
	public void Refresh();

	/// <summary>
	/// Updates what's being shown for this location, like <see cref="Refresh"/> .
	/// Even if the location is not currently being shown,
	/// you at least have the assurance that
	/// <paramref name="justAdded"/> will be parented to a new <see cref="Godot.Node"/>  after this.
	/// </summary>
	public void RefreshJustAdded(ICardController justAdded);
}