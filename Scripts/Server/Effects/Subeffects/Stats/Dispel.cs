using Kompas.Cards.Movement;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Dispel : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			var card = CardTarget ?? throw new NullCardException(TargetWasNull);
			card.Dispel(Effect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}