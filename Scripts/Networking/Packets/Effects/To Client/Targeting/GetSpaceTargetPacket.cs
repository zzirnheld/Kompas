using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using KompasCore.GameCore;
using System.Linq;

namespace Kompas.Networking.Packets
{
	public class GetSpaceTargetPacket : Packet
	{
		public string cardName;
		public string targetBlurb;
		public int[] possibleSpaces;
		public int[] recommendedSpaces;

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
			clientGame.clientUIController.TargetMode = TargetMode.SpaceTarget;
			//TODO check whether client setting says "yes recommendations" or not
			clientGame.CurrentPotentialSpaces = recommendedSpaces.Select(s => (s / 7, s % 7)).ToArray();
			clientGame.clientUIController.currentStateUIController.ChooseSpaceTarget(cardName, targetBlurb);
		}
	}
}