using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using Kompas.Server.Effects.Controllers;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class Heal : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var target = CardTarget ?? throw new NullCardException(TargetWasNull);
		if (forbidNotBoard && target.Location != Location.Board)
			throw new InvalidLocationException(target.Location, target, ChangedStatsOfCardOffBoard);
		if (target.E >= target.BaseE)
			throw new InvalidCardException(target, TooMuchEForHeal);

		int healedFor = target.BaseE - target.E;
		var contexts = IEventContext.Build(Trigger.Healed)
			.PrimarilyAffecting(target)
			.CausedBy(Effect)
			.ForPlayer(PlayerTarget)
			.WithX(healedFor)
			.Capture(() => target.SetE(target.BaseE, stackSrc: ServerEffect));
		ServerEffect.EffectsController.TriggerFor(contexts);
		return Task.FromResult(ResolutionInfo.Next);
	}
}