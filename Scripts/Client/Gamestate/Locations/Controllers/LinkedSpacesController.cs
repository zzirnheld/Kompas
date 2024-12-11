using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Gamestate;
using Kompas.Shared.Enumerable;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class LinkedSpacesController : Node3D
	{
		private IDictionary<Space, LinkedSpaceController>? _spaces;
		public IDictionary<Space, LinkedSpaceController> Spaces => _spaces
			?? throw new NotReadyYetException();

		[Export]
		private Node? _linkedSpacesParent;
		private Node LinkedSpacesParent => _linkedSpacesParent ?? throw new UnassignedReferenceException();


		public override void _Ready()
		{
			base._Ready();
			_spaces = LinkedSpacesParent.GetChildren()
				.CastOrReject<Node, LinkedSpaceController>()
				.ToDictionary(lsc => lsc.Coords);

			if (Spaces.Values.Any(s => s.GetParent() != LinkedSpacesParent)) Logger.Err($"{Name} AAAAAAAAAAAAA");

			//for testing
			//Display(s => s.DistanceTo((1, 2)) <= 1, true);
		}

		public delegate bool ShouldShowSpace(Space space);

		public void Display(ShouldShowSpace predicate, bool showConnections)
		{
			//Logger.Log("Displaying linked spaces!");
			foreach (var space in Spaces.Values) space.DisplayNone();

			ISet<Space> shown = Space.Spaces
				.Where(predicate.Invoke)
				.ToHashSet();

			foreach (var space in shown)
			{
				var linkedSpace = Spaces[space];
				bool showPlusX = showConnections && shown.Contains(space + (1, 0));
				bool showPlusY = showConnections && shown.Contains(space + (0, 1));
				linkedSpace.Display(showPlusX, showPlusY);
			}
		}

		public void UpdateMaterial(Material material)
		{
			foreach (var space in Spaces.Values) space.UpdateMaterial(material);
		}

		public void UpdateTransparency(float t)
		{
			foreach (var space in Spaces.Values) space.UpdateTransparency(t);
		}
	}
}