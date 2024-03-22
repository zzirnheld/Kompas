using Godot;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.Gamestate.Locations.Controllers
{
	public abstract partial class HandController : Node
	{
		private IHand? _handModel;
		public IHand HandModel
		{
			get => _handModel ?? throw new UnassignedReferenceException();
			set => _handModel = value;
		}

		public void Refresh() => SpreadAllCards();

		protected abstract void SpreadAllCards();
	}
}