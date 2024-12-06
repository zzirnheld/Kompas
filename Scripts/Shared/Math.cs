namespace Kompas.Shared;

public static class Math
{
	/// <summary>
	/// Input and output both bound [0, 1]
	/// </summary>
	public delegate float ProgressToProportion(float progress);
	public static readonly ProgressToProportion CubicProgress = Cubic;

	/// <summary>
	/// Cubic interpolation smoothing between 0 and 1.
	///</summary>
	public static float Cubic(float x) => 6 * ((x * x / 2) - (x * x * x / 3));
}