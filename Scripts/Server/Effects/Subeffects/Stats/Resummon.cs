using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using Kompas.Server.Effects.Controllers;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class Resummon : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve()
	{
		var target = CardTarget ?? throw new NullCardException(TargetWasNull);
		if (forbidNotBoard && target.Location != Location.Board)
			throw new InvalidLocationException(target.Location, target, "Target not on board :(");

		var contexts = IEventContext.Build(Trigger.Play)
			.PrimarilyAffecting(target)
			.CausedBy(Effect)
			.ForPlayer(PlayerTarget)
			.At(target.Position)
			.Capture(() => { },
				ctxt => ctxt,
				ctxt => ctxt.CloneForEvent(Trigger.Arrive));
		ServerGame.StackController.TriggerFor(contexts);

		return Task.FromResult(ResolutionInfo.Next);
	}
}