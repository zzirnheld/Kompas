using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;
using Godot;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetTriggeringCardsSpace : ServerSubeffect
	{
		public bool after = false;

		public override Task<ResolutionInfo> Resolve()
		{
			var cardInfo = (after
				? ResolutionContext.TriggerContext?.MainCardInfoAfter
				: ResolutionContext.TriggerContext?.MainCardInfoBefore)
				?? throw new NullCardException(TargetWasNull);
			if (cardInfo.Location != Location.Board) throw new InvalidCardException(cardInfo.Card, $"Card wasn't on board at the time!");
			if (cardInfo.Position == null) throw new NullSpaceOnBoardException(cardInfo.Card);
			if (!cardInfo.Position.IsValid) throw new InvalidSpaceException(cardInfo.Position, NoValidSpaceTarget);

			ServerEffect.AddSpace(cardInfo.Position.Copy);
			GD.Print($"Just added {SpaceTarget} from {cardInfo}");
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}