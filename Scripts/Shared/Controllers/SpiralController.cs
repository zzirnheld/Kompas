using System.Collections.Generic;
using Godot;
using Kompas.Godot;
using Kompas.Shared.Enumerable;

namespace Kompas.Shared.Controllers
{
	/// <summary>
	/// Naive, stupid, ugly placeholder so I can start thinking about and testing game functionality.
	/// To be replaced later with a solution that considers the visual elements.
	/// </summary>
	public partial class SpiralController : Node3D
	{
		private const float AngleStepDivisor = 12f;

		[Export]
		private float ObjectHeight { get; set; }
		[Export]
		private float ObjectDiameter { get; set; }
		[Export]
		private float SpiralTighten { get; set; }
		[Export]
		private float StartingAngle { get; set; }

		//TODO - this means it will not itself handle removing a child, unless I do some magic with events
		public void SpiralOut(IReadOnlyCollection<Node3D> children)
		{
			float angle = StartingAngle;
			foreach (var (index, child) in children.Enumerate())
			{
				angle = CalculateNextAngle(index, angle);
				var (x, y) = CalculateCoordinates(angle);
				this.TransferChild(child);
				child.Position = new(x, ObjectHeight, y);
			}
		}

		private float CalculateNextAngle(int index, float lastAngle)
		{
			float step = Mathf.Pi / ((index + 1f) * AngleStepDivisor); //don't divide by 0
			float lastRadius = CalculateRadius(lastAngle);

			float nextAngle = lastAngle;
			float chordLength = 0;
			while (chordLength < ObjectDiameter)
			{
				nextAngle += step;
				var nextRadius = CalculateRadius(nextAngle);
				//cosine rule
				chordLength = (lastRadius * lastRadius) + (nextRadius * nextRadius) - (2 * lastRadius * nextRadius * Mathf.Cos(nextAngle - lastAngle));

				if (nextAngle > 1000f)
				{
					Logger.Err("TOO far!");
					break;
				}
			}

			Logger.Log($"Item {index} goes at angle {nextAngle}");

			return nextAngle;
		}

		private float CalculateRadius(float angle) => angle * ObjectDiameter * SpiralTighten;

		private (float, float) CalculateCoordinates(float angle)
		{
			var radius = CalculateRadius(angle);
			float x = radius * Mathf.Sin(angle);
			float y = radius * Mathf.Cos(angle);
			return (x, y);
		}
	}
}