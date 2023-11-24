using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using System.Linq;
using Kompas.Gamestate;
using Godot;

namespace Kompas.Networking.Packets
{
	public class GetSpaceTargetPacket : Packet
	{
		public string? cardName;
		public string? targetBlurb;
		public int[]? possibleSpaces; //storing a space as a single int. I originally did this for space + serialization worries. probably not worth worrying about tho
		public int[]? recommendedSpaces;

		public GetSpaceTargetPacket() : base(GetSpaceTarget) { }

		public GetSpaceTargetPacket(string cardName, string targetBlurb, (int x, int y)[] possibleSpaces, (int x, int y)[] recommendedSpaces) : this()
		{
			this.cardName = cardName;
			this.targetBlurb = targetBlurb;
			this.possibleSpaces = possibleSpaces.Select(s => s.x * 7 + s.y).ToArray();
			this.recommendedSpaces = recommendedSpaces.Select(s => s.x * 7 + s.y).ToArray();
		}
	}
}

namespace Kompas.Client.Networking
{
	public class GetSpaceTargetClientPacket : GetSpaceTargetPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			if (cardName == null || targetBlurb == null || possibleSpaces == null || recommendedSpaces == null)
			{
				GD.PushWarning("Missing something in get space target packet");
				return;
			}
			clientGame.ClientGameController.TargetingController
				.StartSpaceSearch(recommendedSpaces.Select(s => new Space(s / 7, s % 7)), targetBlurb);
		}
	}
}