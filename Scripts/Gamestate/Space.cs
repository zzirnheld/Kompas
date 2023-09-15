using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kompas.Gamestate
{
public class Space
{
	public const int BoardLen = 7;
	public const int MaxIndex = BoardLen - 1;
	public static readonly Space Invalid = (-1, -1);

	public int x;
	public int y;

	public Space(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public class SpaceCollection : IEnumerable<Space>
	{
		public IEnumerator<Space> GetEnumerator()
		{
			for (int x = 0; x < BoardLen; x++)
			{
				for (int y = 0; y < BoardLen; y++)
				{
					yield return (x, y);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}
	}
	public static SpaceCollection Spaces => new SpaceCollection();

	//TODO: have the space -> world space calculation here or in board ctrl? regardless, need to add it to the following places:
	//where space cue controllers are (so can click them)
	//board control (so can place cards appropriately) GridIndicesToCardPos method.

	public Space Copy => new Space(x, y);

	public static Space NearCorner => new Space(0, 0);
	public static Space FarCorner => new Space(MaxIndex, MaxIndex);
	public static Space Nowhere => new Space(-69, -420);
	public static Space AvatarCornerFor(int index) => index == 0 ? NearCorner : FarCorner;

	public bool IsValid => x >= 0 && y >= 0 && x < BoardLen && y < BoardLen;
	public bool IsCorner => (x == 0 || x == MaxIndex) && (y == 0 || y == MaxIndex);
	public bool IsEdge => x == 0 || x == MaxIndex || y == 0 || y == MaxIndex;

	public int Index => BoardLen * x + y;
	public Space Inverse => new Space(MaxIndex - x, MaxIndex - y);

	public static bool IsValidSpace(int x, int y) => new Space(x, y).IsValid;

	public int TaxicabDistanceTo(Space other) => Math.Abs(x - other.x) + Math.Abs(y - other.y);
	public int RadiusDistanceTo(Space other) => Math.Max(Math.Abs(x - other.x), Math.Abs(y - other.y));
	public int DistanceTo(Space other) => TaxicabDistanceTo(other);
	public Space DisplacementTo(Space other) => new Space(other.x - x, other.y - y);

	public bool IsAdjacentTo(Space other) => DistanceTo(other) == 1;
	public IReadOnlyCollection<Space> AdjacentSpaces
	{
		get
		{
			List<Space> list = new List<Space>();
			var offsets = new int[] { -1, 1 };
			var x = this.x;
			var y = this.y;
			var xs = offsets.Select(o => o + x);
			var ys = offsets.Select(o => o + y);
			foreach (var xCoord in xs)
			{
				Space s = (xCoord, y);
				if (s.IsValid) list.Add(s);
			}
			foreach (var yCoord in ys)
			{
				Space s = (x, yCoord);
				if (s.IsValid) list.Add(s);
			}
			//GD.Print($"Spacesadjacent to {this} are {string.Join(", ", list.Select(s => s.ToString()))}");
			return list;
		}
	}

	public bool SameColumn(Space other) => x - y == other.x - other.y;
	public bool SameDiagonal(Space other) => x == other.x || y == other.y;
	public bool SameAxis(Space a, Space b)
		=> (((a.x - b.x) % (a.x - x) == 0) || ((a.x - x) % (a.x - b.x) == 0))
		&& (((a.y - b.y) % (a.y - y) == 0) || ((a.y - y) % (a.y - b.y) == 0));

	public bool NorthOf(Space other) => x > other.x || y > other.y;
	public Space DueNorth => new Space(x + 1, y + 1);

	/// <summary>
	/// Returns the space directly between this space and the other one
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public Space DirectlyBetween(Space other)
	{
		if (!SameDiagonal(other)) return default;
		else return AdjacentSpaces.Intersect(other.AdjacentSpaces).FirstOrDefault();
	}

	/// <summary>
	/// Whether this space is between the two others.
	/// "Between" is ill-defined for 3 spaces not on the same diagonal.
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	public bool Between(Space a, Space b)
	{
		if (a == null) throw new ArgumentNullException("a", "First space of 'between' was null");
		if (b == null) throw new ArgumentNullException("b", "Second space of 'between' was null");

		if (a.x == b.x && b.x == x) return (a.y > y && b.y < y) || (a.y < y && b.y > y);
		if (a.y == b.y && b.y == y) return (a.x > x && b.x < x) || (a.x < x && b.x > x);

		//If not on either of the same diagonals, "between" is ill-defined.
		return false;
	}

	public Space DirectionFromThisTo(Space other)
	{
		(int x, int y) diff = (other.x - x, other.y - y);
		if (diff.x == 0 || diff.y == 0) return (Math.Sign(diff.x), Math.Sign(diff.y));
		else if (diff.x % diff.y == 0) return (diff.x / diff.y, 1);
		else if (diff.y % diff.x == 0) return (1, diff.y / diff.x);
		else return diff;
	}

	public Space GeneralDirectionFromThisTo(Space other)
	{
		(int x, int y) diff = (x - other.x, y - other.y);
		if (diff.x == 0 || diff.y == 0) return (Math.Sign(diff.x), Math.Sign(diff.y));
		else if (diff.x % diff.y == 0) return (Math.Sign(diff.x) * (diff.x / diff.y), Math.Sign(diff.y));
		else if (diff.y % diff.x == 0) return (Math.Sign(diff.x), Math.Sign(diff.y) * (diff.y / diff.x));
		else return diff;
	}

	public static Space operator *(Space s, int i) => (s.x * i, s.y * i);
	public static Space operator +(Space a, Space b) => (a.x + b.x, a.y + b.y);

	public static bool operator ==(Space a, Space b)
	{
		//GD.Print($"Comparing {a} to {b}");
		if (a is null) return b is null;
		else if (b is null) return false;
		else return a.x == b.x && a.y == b.y;
	}
	public static bool operator !=(Space a, Space b) => !(a == b);
	public static bool operator ==(Space a, (int x, int y) b) => a != null && a.x == b.x && a.y == b.y;
	public static bool operator !=(Space a, (int x, int y) b) => !(a == b);
	public static bool operator ==((int x, int y) a, Space b) => b != null && a.x == b.x && a.y == b.y;
	public static bool operator !=((int x, int y) a, Space b) => !(a == b);

	public static implicit operator (int x, int y)(Space space) => (space.x, space.y);
	public static implicit operator Space((int x, int y) s) => new Space(s.x, s.y);

	public void Deconstruct(out int xCoord, out int yCoord)
	{
		xCoord = x;
		yCoord = y;
	}

	public override bool Equals(object obj) => obj is Space spc && x == spc.x && y == spc.y;
	public override string ToString() => $"{x}, {y}";
	public override int GetHashCode() => x + BoardLen * y;
}
}