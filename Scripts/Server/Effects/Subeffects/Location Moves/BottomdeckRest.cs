using System.Threading.Tasks;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public class BottomdeckRestData : SubeffectData { }

public class BottomdeckRest : ServerSubeffect
{
	public BottomdeckRest(BottomdeckRestData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		GetPlayerTarget(resolution.Context).Deck.BottomdeckMany(resolution.Context.Rest);

		return Task.FromResult(ResolutionInfo.Next);
	}
}