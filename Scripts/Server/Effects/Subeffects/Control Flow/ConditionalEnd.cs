using Kompas.Effects.Models.Restrictions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class ConditionalEnd : ServerSubeffect
	{
		public IGamestateRestriction endIfTrue;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			endIfTrue.Initialize(DefaultInitializationContext);
		}

		public override Task<ResolutionInfo> Resolve()
		{
			//TODO implement a ToHumanReadableString sort of thing to provide as a reason here
			if (endIfTrue.IsValid(ResolutionContext)) return Task.FromResult(ResolutionInfo.End("I said so"));
			else return Task.FromResult(ResolutionInfo.Next);
		}
	}
}