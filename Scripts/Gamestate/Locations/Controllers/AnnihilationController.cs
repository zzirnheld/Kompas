using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.Gamestate.Locations.Controllers;

public partial class AnnihilationController : Node, IAnnihilationController //TODO shared parent class for location controllers? similar to models?
{
	private Annihilation? _annihilationModel;
	public Annihilation AnnihilationModel
	{
		get => _annihilationModel ?? throw new UnassignedReferenceException();
		set => _annihilationModel = value;
	}

	public virtual void Refresh() { }
	public void RefreshJustAdded(ICardController cardController) => Refresh(); //TODO
}

public interface IAnnihilationController : IRefreshableLocationController
{
	public Annihilation AnnihilationModel { get; set; }
}