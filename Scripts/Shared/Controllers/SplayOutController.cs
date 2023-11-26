using System;
using System.Collections.Generic;
using Godot;
using Kompas.Shared.Enumerable;

namespace Kompas.Shared.Controllers
{
	/// <summary>
	/// Naive, stupid, ugly placeholder so I can start thinking about and testing game functionality.
	/// To be replaced later with a solution that considers the visual elements.
	/// </summary>
	public partial class SplayOutController : Node3D
	{
		public const float Offset = 2.25f;

		[Export]
		private int HorizontalGrowDirection { get; set; }
		[Export]
		private int VerticalGrowDirection { get; set; }

		//TODO - this means it will not itself handle removing a child, unless I do some magic with events
		public void SplayOut(IReadOnlyCollection<Node3D> children)
		{
			var count = children.Count;

			//r rows can fit (r* (r+1))/2 items
			//therefore, n items can be fit into (-1 + sqrt(8n + 1))/2 rows (invert it and use the quadratic formula)
			int colCount = Mathf.CeilToInt((Mathf.Sqrt(8f * count + 1f) - 1f) / 2f);

			int row = 0, col = 0;

			foreach (var (index, newChild) in children.Enumerate())
			{
				newChild.GetParent()?.RemoveChild(newChild);
				AddChild(newChild);

				newChild.Position = (Vector3.Right * HorizontalGrowDirection * col) + (Vector3.Back * VerticalGrowDirection * row);
				col++;
				if (col > colCount)
				{
					col = 0;
					row++;
					colCount--;
				}
			}
		}
	}
}