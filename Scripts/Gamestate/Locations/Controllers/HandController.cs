using Godot;
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

	protected abstract void SpreadAllCards();
}

public interface IHandController
{
	public Hand HandModel { get; set; }
	public void Refresh();
}