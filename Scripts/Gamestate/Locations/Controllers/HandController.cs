using Godot;
using Kompas.Cards.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.Gamestate.Locations.Controllers;

public abstract partial class HandController : Node, IHandController
{
	private Hand? _handModel;
	public Hand HandModel
	{
		get => _handModel ?? throw new UnassignedReferenceException();
		set => _handModel = value;
	}

	public void Refresh() => SpreadAllCards();
	public void RefreshJustAdded(ICardController cardController) => Refresh();

	protected abstract void SpreadAllCards();
}

public interface IHandController : IRefreshableLocationController
{
	public Hand HandModel { get; set; }
}