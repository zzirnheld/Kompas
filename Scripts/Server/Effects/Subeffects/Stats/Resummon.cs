using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using Kompas.Server.Effects.Controllers;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ResummonData : SubeffectData { }

/// <summary>
/// Known these days as echoing, but changing the JSON is annoying
/// </summary>
public class Resummon : ServerSubeffect
{
	public Resummon(ResummonData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var target = GetCardTarget(resolution.Context);
		if (forbidNotBoard && target.Location != Location.Board)
			throw new InvalidLocationException(target.Location, target, "Target not on board :(");

		var contexts = IEventContext.Build(Trigger.Play)
			.PrimarilyAffecting(target)
			.CausedBy(Effect)
			.ForPlayer(GetPlayerTarget(resolution.Context))
			.At(target.Position)
			.Capture(() => { },
				ctxt => ctxt,
				ctxt => ctxt.CloneForEvent(Trigger.Arrive));
		ServerGame.StackController.TriggerFor(contexts);

		return Task.FromResult(ResolutionInfo.Next);
	}
}