using Godot;
using Kompas.Cards.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.Gamestate.Locations.Controllers;

public abstract partial class DiscardController : Node, IDiscardController
{
	private Discard? _discardModel;
	public Discard DiscardModel
	{
		get => _discardModel ?? throw new UnassignedReferenceException();
		set => _discardModel = value;
	}

	public void Refresh() => SpreadOut();
	public abstract void RefreshJustAdded(ICardController cardController);

	protected abstract void SpreadOut();
}

public interface IDiscardController : IRefreshableLocationController
{
	public Discard DiscardModel { get; set; }
}