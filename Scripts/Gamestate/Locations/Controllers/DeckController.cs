using Godot;
using Kompas.Cards.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.Gamestate.Locations.Controllers;

public abstract partial class DeckController : Node, IDeckController //TODO shared parent class for location controllers? similar to models?
{
	private Deck? _deckModel;
	public Deck DeckModel
	{
		get => _deckModel ?? throw new UnassignedReferenceException();
		set => _deckModel = value;
	}

	public void Refresh() => SpreadOut();
	public void Remove(ICardController cardController)
	{
		cardController.Hide();
		Refresh();
	}

	protected abstract void SpreadOut();
}

public interface IDeckController : ILocationController
{
	public Deck DeckModel { get; set; }
}