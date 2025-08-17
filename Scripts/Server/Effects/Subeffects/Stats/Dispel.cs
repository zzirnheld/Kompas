using Kompas.Cards.Movement;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class DispelData : SubeffectData { }

public class Dispel : ServerSubeffect
{
	public Dispel(DispelData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var card = GetCardTarget(resolution.Context) ?? throw new NullCardException(TargetWasNull);
		card.Dispel(Effect);
		return Task.FromResult(ResolutionInfo.Next);
	}
}