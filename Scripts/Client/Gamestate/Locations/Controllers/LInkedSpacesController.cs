using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Client.Networking;
using Kompas.Gamestate;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class LinkedSpacesController : Node3D
	{
		private IDictionary<Space, LinkedSpaceController>? _spaces;
		public IDictionary<Space, LinkedSpaceController> Spaces => _spaces
			?? throw new NotReadyYetException();


		public override void _Ready()
		{
			base._Ready();
			_spaces = GetChildren()
				.Where(child => child is LinkedSpaceController)
				.Cast<LinkedSpaceController>()
				.ToDictionary(lsc => lsc.Coords);
		}

		public delegate bool ShouldShowSpace(Space space);

		public void Display(ShouldShowSpace predicate)
		{
			foreach (var space in Space.Spaces)
			{
				if (predicate(space));
				//TODO need to know whether this space *and* the ones to its top left. so... go from furthest top left, then down? 6,6 , then 5, 6, then 6, 5, then 5, 4, 5, 5 4, 5?
			}
		}
	}
}