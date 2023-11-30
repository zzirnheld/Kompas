using System.Collections.Generic;
using Godot;
using Kompas.Shared.Enumerable;

namespace Kompas.Shared.Controllers
{
	/// <summary>
	/// Naive, stupid, ugly placeholder so I can start thinking about and testing game functionality.
	/// To be replaced later with a solution that considers the visual elements.
	/// </summary>
	public partial class SpiralController : Node3D
	{
		private static readonly float FourOverPi = 4 / Mathf.Pi;

		[Export]
		private float ObjectHeight { get; set; } = 0.05f;
		[Export]
		private float ObjectRadius { get; set; } = 2.25f;

		//TODO - this means it will not itself handle removing a child, unless I do some magic with events
		public void SpiralOut(IReadOnlyCollection<Node3D> children)
		{
			foreach (var (index, child) in children.Enumerate())
			{
				var (x, y) = CalculateCoordinates(index);
				child.GetParent()?.RemoveChild(child);
				AddChild(child);
				child.Position = new(x, ObjectHeight, y);
			}
		}

		private (float, float) CalculateCoordinates(int index)
		{
			float radius = CalculateSpiralRadius(index);
			float angle = index; //in radians

			float x = radius * Mathf.Sin(angle);
			float y = radius * Mathf.Cos(angle);
			return (x, y);
		}

		private float CalculateSpiralRadius(int itemIndex)
		{
			//radius = (1 + sqrt(4/pi * index + 1))/2 - 1
			float underSqrt = FourOverPi * itemIndex + 1f;
			float radiusRaw = (1f + Mathf.Sqrt(underSqrt)) / 2f - 1f;
			float radius = radiusRaw * ObjectRadius;
			return radius;
		}
	}
}