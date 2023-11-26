using System.Threading.Tasks;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Activate : ServerSubeffect
	{
		public bool activate = true;

		public override Task<ResolutionInfo> Resolve()
		{
			var card = CardTarget ?? throw new NullCardException(TargetWasNull);
			card.SetActivated(activate, Effect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}