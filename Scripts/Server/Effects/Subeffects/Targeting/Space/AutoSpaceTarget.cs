using Kompas.Effects.Models;
using System.Linq;
using System.Threading.Tasks;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class AutoSpaceTarget : ServerSubeffect
{
	#nullable disable
	[JsonProperty(Required = Required.Always)]
	public IRestriction<Space> spaceRestriction;
	#nullable restore

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);

		spaceRestriction.Initialize(DefaultInitializationContext);
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		try
		{
			Space potentialTarget = Space.Spaces.Single(s => spaceRestriction.IsValid(s, resolution.Context));
			resolution.AddSpace(potentialTarget);
			return Task.FromResult(ResolutionInfo.Next);
		}
		catch (System.InvalidOperationException ioe)
		{
			Logger.Err($"Zero, or more than one space fit the space restriction {spaceRestriction} " +
				$"for the effect {Effect.InitialBlurb} of {Effect.Card?.CardName}. Exception {ioe}");
			return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
		}
	}
}