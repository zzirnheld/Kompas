using System;
using System.Collections.Generic;
using Godot;
using Kompas.Godot;
using Kompas.Shared.Enumerable;
using Kompas.Shared.Exceptions;

namespace Kompas.Shared.Controllers
{
	/// <summary>
    /// Organizes objects in a grid.
	/// </summary>
	public partial class GridController : Node3D
	{
		[Export]
		private Node3D? _nodeParent;
		private Node3D NodeParent => _nodeParent
			?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _topLeftCorner;
		private Node3D TopLeftCorner => _topLeftCorner
			?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _rightBound;
		private Node3D RightBound => _rightBound
			?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _bottomBound;
		private Node3D BottomBound => _bottomBound
			?? throw new UnassignedReferenceException();

		[Export]
		private float MinObjectSize { get; set; } = 1.1f;

		private int rows;
		private int columns;
		private float objectSize;

		public override void _Ready()
		{
			base._Ready();
			float xWidth = (RightBound.Position.X - TopLeftCorner.Position.X);
			float zWidth = (BottomBound.Position.Z - TopLeftCorner.Position.Z);
			columns = Mathf.FloorToInt(xWidth / MinObjectSize) + 1; //because we want to fully cover that area
			rows = Mathf.FloorToInt(zWidth / MinObjectSize); //because we want to fully cover that area
			objectSize = xWidth / columns;
		}

		public void Arrange(IReadOnlyCollection<Node3D> nodes)
		{
			int col = 0;
			int row = 0;
			foreach (var (index, node) in nodes.Enumerate())
			{
				NodeParent.TransferChild(node);
				node.Visible = true;
				node.Scale = Vector3.One;
				node.Position = TopLeftCorner.Position + new Vector3(col * objectSize, 0.05f, row * objectSize);
				col++;
				if (col >= columns)
				{
					col = 0;
					row++;
				}
			}
		}
	}
}