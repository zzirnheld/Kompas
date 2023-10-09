using KompasCore.Cards;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class StashTargetsCardInfo : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null) throw new NullCardException(NoValidCardTarget);

			ServerEffect.CardInfoTargets.Add(GameCardInfo.CardInfoOf(CardTarget));
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}
