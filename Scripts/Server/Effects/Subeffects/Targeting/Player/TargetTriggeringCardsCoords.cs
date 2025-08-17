using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;
using Kompas.Gamestate.Locations;
using Kompas.Effects.Subeffects;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTriggeringCardsSpaceData : SubeffectData
{
	[JsonProperty]
	public bool after;
}

public class TargetTriggeringCardsSpace : ServerSubeffect
{
	private readonly bool after;

	public TargetTriggeringCardsSpace(TargetTriggeringCardsSpaceData data) : base(data)
	{
		after = data.after;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var cardInfo = (after
			? resolution.Context.TriggerContext?.MainCardAfter
			: resolution.Context.TriggerContext?.MainCardBefore)
			?? throw new NullCardException(TargetWasNull);
		if (cardInfo.Location != Location.Board) throw new InvalidCardException(cardInfo.Card, $"Card wasn't on board at the time!");
		if (cardInfo.Position == null) throw new NullSpaceOnBoardException(cardInfo.Card);
		if (!cardInfo.Position.IsValid) throw new InvalidSpaceException(cardInfo.Position, NoValidSpaceTarget);

		resolution.AddSpace(cardInfo.Position.Copy);
		Logger.Log($"Just added {GetSpaceTarget(resolution.Context)} from {cardInfo}");
		return Task.FromResult(ResolutionInfo.Next);
	}
}