using Kompas.Effects.Models;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class AutoSpaceTarget : ServerSubeffect
	{
		public IRestriction<Space> spaceRestriction;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);

			spaceRestriction.Initialize(DefaultInitializationContext);
		}

		public override Task<ResolutionInfo> Resolve()
		{
			try
			{
				Space potentialTarget = Space.Spaces.Single(s => spaceRestriction.IsValid(s, ResolutionContext));
				ServerEffect.AddSpace(potentialTarget);
				return Task.FromResult(ResolutionInfo.Next);
			}
			catch (System.InvalidOperationException ioe)
			{
				GD.PrintErr($"Zero, or more than one space fit the space restriction {spaceRestriction} " +
					$"for the effect {Effect.blurb} of {Card.CardName}. Exception {ioe}");
				return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
			}
		}
	}
}