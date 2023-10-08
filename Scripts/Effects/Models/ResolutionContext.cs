using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	public class ResolutionContext : IResolutionContext
	{
		public TriggeringEventContext TriggerContext { get; }

		// Used for resuming delayed effects
		public int StartIndex { get; }
		public IList<GameCard> CardTargets { get; }
		public IList<GameCardInfo> CardInfoTargets { get; }
		public GameCard DelayedCardTarget { get; }
		public IList<Space> SpaceTargets { get; }
		public Space DelayedSpaceTarget { get; }
		public IList<IStackable> StackableTargets { get; }
		public IStackable DelayedStackableTarget { get; }

		public int X { get; set; }

		public static ResolutionContext PlayerTrigger(Effect effect, IGame game)
			=> new(new TriggeringEventContext(game: game, stackableEvent: effect));

		public ResolutionContext(TriggeringEventContext triggerContext)
		: this(triggerContext, 0,
			Enumerable.Empty<GameCard>(), default,
			Enumerable.Empty<GameCardInfo>(),
			Enumerable.Empty<Space>(), default,
			Enumerable.Empty<IStackable>(), default)
		{ }

		public ResolutionContext(TriggeringEventContext triggerContext,
			int startIndex,
			IEnumerable<GameCard> cardTargets, GameCard delayedCardTarget,
			IEnumerable<GameCardInfo> cardInfoTargets,
			IEnumerable<Space> spaceTargets, Space delayedSpaceTarget,
			IEnumerable<IStackable> stackableTargets, IStackable delayedStackableTarget)
		{
			TriggerContext = triggerContext;
			StartIndex = startIndex;

			CardTargets = Clone(cardTargets);
			DelayedCardTarget = delayedCardTarget;

			CardInfoTargets = Clone(cardInfoTargets);

			SpaceTargets = Clone(spaceTargets);
			DelayedSpaceTarget = delayedSpaceTarget;

			StackableTargets = Clone(stackableTargets);
			DelayedStackableTarget = delayedStackableTarget;

			X = TriggerContext?.x ?? 0;
		}

		private static List<T> Clone<T>(IEnumerable<T> list)
		{
			if (list == null) return new List<T>();
			else return new List<T>(list);
		}

		public IResolutionContext Copy => new ResolutionContext(TriggerContext, StartIndex,
			CardTargets, DelayedCardTarget,
			CardInfoTargets,
			SpaceTargets, DelayedSpaceTarget,
			StackableTargets, DelayedStackableTarget);

		public override string ToString()
		{
			var sb = new System.Text.StringBuilder();
			sb.Append(base.ToString());
			sb.Append(TriggerContext?.ToString());

			if (CardTargets != null) sb.Append($"Targets: {string.Join(", ", CardTargets)}, ");
			if (StartIndex != 0) sb.Append($"Starting at {StartIndex}");

			return sb.ToString();
		}
	}
}