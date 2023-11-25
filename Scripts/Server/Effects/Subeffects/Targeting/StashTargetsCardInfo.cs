using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class StashTargetsCardInfo : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null) throw new NullCardException(NoValidCardTarget);

			var targetInfo = GameCardInfo.CardInfoOf(CardTarget)
				?? throw new System.InvalidOperationException("Failed to create a card info!");
			ServerEffect.CardInfoTargets.Add(targetInfo);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}
