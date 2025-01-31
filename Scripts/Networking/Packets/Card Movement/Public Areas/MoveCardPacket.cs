using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using Kompas.Cards.Movement;
using Kompas.Gamestate;
using System.Linq;
using Kompas.Shared.Enumerable;
using System.Collections.Generic;

namespace Kompas.Networking.Packets
{
	public class MoveCardPacket : Packet
	{
		public int cardId;
		public int x;
		public int y;

		public int[] path;

		public MoveCardPacket() : base(MoveCard) { }

		private MoveCardPacket(int cardId, int x, int y, int[] path, bool invert = false)
			: this()
		{
			this.cardId = cardId;
			this.x = invert ? Space.MaxIndex - x : x;
			this.y = invert ? Space.MaxIndex - y : y;

			this.path = path;
			if (invert)
			{
				for (int i = 0; i < path.Length; i++)
				{
					path[i] = Space.FromIndex(path[i]).Inverse.Index;
				}
			}
		}

		public MoveCardPacket(int cardId, int x, int y, bool invert, MovePath path)
			: this(
				cardId, x, y,
				MovePathToIntArray(path),
				invert: invert
			)
		{ }

		private static int[] MovePathToIntArray(MovePath path)
		{
			var ints = new List<int>();
			foreach (var space in path.Spaces) {
				if (space == null) return System.Array.Empty<int>();

				ints.Add(space.Index);
			}
			return ints.ToArray();
		}

		public override Packet Copy() => new MoveCardPacket(cardId, x, y, path);

		public override Packet? GetInversion(bool known) => new MoveCardPacket(cardId, x, y, path, invert: true);
	}
}

namespace Kompas.Client.Networking
{
	public class MoveCardClientPacket : MoveCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var movePath = new MovePath() { Spaces = path.Select(Space.FromIndex).ToArray() };
			Logger.Log($"Moving {cardId} to {x}, {y} via {string.Join(", ", movePath.Spaces)}");
			clientGame.LookupCardByID(cardId)
				?.Move((x, y), normalMove: false, mover: null, path: movePath);
			//TODO have move in client call refresh. for that matter, position change
		}
	}
}