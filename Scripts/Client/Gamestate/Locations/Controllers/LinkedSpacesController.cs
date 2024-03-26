using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Gamestate;
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
				.Where(child => child is LinkedSpaceController)
				.Cast<LinkedSpaceController>()
				.ToDictionary(lsc => lsc.Coords);

			if (Spaces.Values.Any(s => s.GetParent() != LinkedSpacesParent)) Logger.Err($"{Name} AAAAAAAAAAAAA");

			//for testing
			//Display(s => Math.Abs(s.x - 1) + Math.Abs(s.y - 2) <= 2);
		}

		public delegate bool ShouldShowSpace(Space space);

		/*
		private IEnumerable<Space> ProduceSpacesInTopToBottomOrder()
		{
			(int x, int y) = Space.FarCorner;
			for (int offset = 0; offset < Space.BoardLen; offset++)
			{
				//when offset = 1, iterate for xOffset = 0, 1
				for (int xOffset = 0; xOffset <= offset; xOffset++)
				{
					int yOffset = offset - xOffset;
					yield return (x - xOffset, y - yOffset);
				}
			}
			for (int offset = Space.BoardLen; offset < 2 * Space.BoardLen; offset++)
			{
				//when offset = 7, need to have xoffset start at 6 and yOffset start at 1;
				for (int xOffset = Space.BoardLen - 1; xOffset > offset - Space.BoardLen; xOffset--)
				{
					int yOffset = offset - xOffset;
					yield return (x - xOffset, y - yOffset);
				}
			}
		}

		public void Display(ShouldShowSpace predicate)
		{
			//this is an entirely unnecessary dynamic programming approach that I did purely as a mental exercise.
			//it's technically probably faster? but this is not a performance-intensive thing and so I'll stick with correctness
			int[,] filled = new int[Space.BoardLen, Space.BoardLen];

			foreach (var space in ProduceSpacesInTopToBottomOrder())
			{
				if (predicate(space));
			}
		}*/

		public void Display(ShouldShowSpace predicate, bool showConnections)
		{
			foreach (var space in Spaces.Values) space.DisplayNone();

			ISet<Space> shown = Space.Spaces
				.Where(s => predicate(s))
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