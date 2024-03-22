using Godot;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.Gamestate.Locations.Controllers
{
	public abstract partial class DiscardController : Node //TODO shared parent class for location controllers? similar to models?
	{
		private IDiscard? _discardModel;
		public IDiscard DiscardModel
		{
			get => _discardModel ?? throw new UnassignedReferenceException();
			set => _discardModel = value;
		}

		public void Refresh() => SpreadOut();

		protected abstract void SpreadOut();
	}
}