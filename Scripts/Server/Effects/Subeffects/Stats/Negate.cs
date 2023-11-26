using System.Threading.Tasks;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Negate : ServerSubeffect
	{
		public bool negated = true;

		public override Task<ResolutionInfo> Resolve()
		{
			var card = CardTarget ?? throw new NullCardException(TargetWasNull);
			card.SetNegated(negated, ServerEffect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}