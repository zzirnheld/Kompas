using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class EndTurn : ServerSubeffect
	{
		public override async Task<ResolutionInfo> Resolve()
		{
			await ServerGame.SwitchTurn();
			return ResolutionInfo.Next;
		}
	}
}