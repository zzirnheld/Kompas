using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kompas.Gamestate
{
	//FUTURE: Possible optimization: make this a struct, and all the current instances nullable.
	public class Space
	{
		public const int BoardLen = 7;
		public const int NoPathExists = int.MaxValue;
		public const int MaxIndex = BoardLen - 1;
		public static readonly Space Invalid = (-69, -420);

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
				for (int x = 0; x < BoardLen; x++) for (int y = 0; y < BoardLen; y++)
				{
					yield return (x, y);
				}
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
		public static SpaceCollection Spaces => new();

		//TODO: have the space -> world space calculation here or in board ctrl? regardless, need to add it to the following places:
		//where space cue controllers are (so can click them)
		//board control (so can place cards appropriately) GridIndicesToCardPos method.

		public Space Copy => new(x, y);

		public static readonly Space NearCorner = (0, 0);
		public static readonly Space FarCorner = (MaxIndex, MaxIndex);
		public static readonly Space Nowhere = (-69, -420);
		public static Space AvatarCornerFor(int index) => index == 0 ? NearCorner : FarCorner;

		public bool IsValid => x >= 0 && y >= 0 && x < BoardLen && y < BoardLen;
		public bool IsCorner => (x == 0 || x == MaxIndex) && (y == 0 || y == MaxIndex);
		public bool IsEdge => x == 0 || x == MaxIndex || y == 0 || y == MaxIndex;

		public int Index => BoardLen * x + y;
		public Space Inverse => (MaxIndex - x, MaxIndex - y);

		public static bool IsValidSpace(int x, int y) => new Space(x, y).IsValid;

		public int TaxicabDistanceTo(Space other) => Math.Abs(x - other.x) + Math.Abs(y - other.y);
		public int RadiusDistanceTo(Space other) => Math.Max(Math.Abs(x - other.x), Math.Abs(y - other.y));
		public int DistanceTo(Space other) => TaxicabDistanceTo(other);
		public Space DisplacementTo(Space other) => (other.x - x, other.y - y);

		public static int DistanceBetween(Space? start, Space? destination, Predicate<Space> through)
			=> start?.DistanceTo(destination, through) ?? NoPathExists;

		/// <summary>
		/// A really bad Dijkstra's because this is a fun side project and I'm not feeling smart today
		/// </summary>
		/// <param name="start">The card to start looking from</param>
		/// <param name="x">The x coordinate you want a distance to</param>
		/// <param name="y">The y coordinate you want a distance to</param>
		/// <param name="throughPredicate">What all cards you go through must fit</param>
		/// <returns></returns>
		public int DistanceTo(Space? destination, Predicate<Space> throughPredicate)
		{
			if (this == destination) return 0;
			if (destination == null) return NoPathExists;

			int[,] dist = new int[7, 7];
			bool[,] seen = new bool[7, 7];

			var queue = new Queue<Space>();

			queue.Enqueue(this); //TODO add a unit test, then remove one of these enqueues
			dist[x, y] = 0;
			seen[x, y] = true;

			//set up the structures with the source node
			queue.Enqueue(this);

			//iterate until the queue is empty, in which case you'll have seen all connected cards that fit the restriction.
			while (queue.Any())
			{
				//consider the adjacent cards to the next node in the queue
				var curr = queue.Dequeue();
				var (currX, currY) = curr;
				foreach (var next in curr.AdjacentSpaces.Where(throughPredicate.Invoke))
				{
					var (nextX, nextY) = next;
					//if that adjacent card is never seen before, initialize its distance and add it to the structures
					if (!seen[nextX, nextY])
					{
						seen[nextX, nextY] = true;
						queue.Enqueue(next);
						dist[nextX, nextY] = dist[currX, currY] + 1;
					}
					//otherwise, relax its distance if appropriate
					else if (dist[currX, currY] + 1 < dist[nextX, nextY])
						dist[nextX, nextY] = dist[currX, currY] + 1;
				}
			}

			return dist[destination.x, destination.y] <= 0 ? Space.NoPathExists : dist[destination.x, destination.y];
		}

		public bool IsAdjacentTo(Space other) => DistanceTo(other) == 1;
		public IReadOnlyCollection<Space> AdjacentSpaces
		{
			get
			{
				List<Space> list = new();
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
		public Space DueNorth => (x + 1, y + 1);

		/// <summary>
		/// Returns the space directly between this space and the other one
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Space? DirectlyBetween(Space other)
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
			if (a == null) throw new ArgumentNullException(nameof(a), "First space of 'between' was null");
			if (b == null) throw new ArgumentNullException(nameof(b), "Second space of 'between' was null");

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

		public static bool AreConnectedBy(Space? start, Space destination, Predicate<Space> through)
			=> start?.IsConnectedTo(destination, through) ?? false;

		public bool IsConnectedTo(Space? destination, Predicate<Space> by)
		{
			if (destination == null) return false;
			return destination.AdjacentSpaces.Any(destAdj => DistanceTo(destAdj, by) < NoPathExists);
		}

		public static bool AreConnectedCheckPathLength
			(Space? source, Space destination, Predicate<Space> spacePredicate, Predicate<int> distancePredicate)
			=> destination.AdjacentSpaces.Any(destAdj => distancePredicate(DistanceBetween(source, destAdj, spacePredicate)));

		public static Space operator *(Space s, int i) => (s.x * i, s.y * i);
		public static Space operator +(Space a, Space b) => (a.x + b.x, a.y + b.y);

		public static bool operator ==(Space? a, Space? b)
		{
			//GD.Print($"Comparing {a} to {b}");
			if (a is null) return b is null;
			else if (b is null) return false;
			else return a.x == b.x && a.y == b.y;
		}
		public static bool operator !=(Space? a, Space? b) => !(a == b);
		public static bool operator ==(Space a, (int x, int y) b) => a != null && a.x == b.x && a.y == b.y;
		public static bool operator !=(Space a, (int x, int y) b) => !(a == b);
		public static bool operator ==((int x, int y) a, Space b) => b != null && a.x == b.x && a.y == b.y;
		public static bool operator !=((int x, int y) a, Space b) => !(a == b);

		public static implicit operator (int x, int y)(Space space) => (space.x, space.y);
		public static implicit operator Space((int x, int y) s) => new(s.x, s.y);

		public void Deconstruct(out int xCoord, out int yCoord)
		{
			xCoord = x;
			yCoord = y;
		}

		public override bool Equals(object? obj) => obj is Space spc && x == spc.x && y == spc.y;
		public override string ToString() => $"{x}, {y}";
		public override int GetHashCode() => x + BoardLen * y;
	}
}