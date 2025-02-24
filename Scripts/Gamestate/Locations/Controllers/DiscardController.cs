using Godot;
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

	protected abstract void SpreadOut();
}

public interface IDiscardController
{
	public Discard DiscardModel { get; set; }
	public void Refresh();
}