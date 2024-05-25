using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Networking;
using Kompas.Shared.Enumerable;

namespace Kompas.Server.Gamestate.Locations.Models
{
    public class ServerBoard : Board
	{
		private readonly ServerGame serverGame;

		private IServerStackController EffectsController => serverGame.StackController;

		public ServerBoard(BoardController boardController, ServerGame serverGame) : base(boardController)
		{
			this.serverGame = serverGame;
		}

		public override void Play(GameCard toPlay, Space to, IPlayer controller, IStackable? stackSrc = null)
		{
			bool wasKnown = toPlay.KnownToEnemy;

			var playContext = IEventContext.Build(Trigger.Play)
				.PrimarilyAffecting(toPlay)
				.CausedBy(stackSrc)
				.ForPlayer(controller)
				.At(to);
			var contexts = EventCapturer.Capture(() => base.Play(toPlay, to, controller, stackSrc: stackSrc),
				playContext, playContext.CloneForEvent(Trigger.Arrive));

			EffectsController.Trigger(contexts);
			
			if (!toPlay.IsAvatar) ServerNotifier.NotifyPlay(controller, toPlay, to, wasKnown);
		}

        protected override void Swap(GameCard card, Space to, bool normal, IPlayer? mover = null, IStackable? stackSrc = null)
		{
			//TODO make a unit test with the old swap triggering event contexts.
			//calculate distance before doing the swap
			var from = card.Position?.Copy;
			var at = GetCardAt(to);

			var incompletes = GetIncompleteMoveContexts(card, from, to, mover, stackSrc)
				.Concat(GetIncompleteMoveContexts(at, to, from, mover, stackSrc));
			var contexts = EventCapturer.Capture(incompletes,
				() => base.Swap(card, to, normal, mover, stackSrc: stackSrc));

			foreach (var context in contexts) EffectsController.TriggerForCondition(context.TriggeringEvent, context);

			//notify the players
			ServerNotifier.NotifyMove(mover ?? card.OwningPlayer, card, to);
		}

		private IEnumerable<IIncompleteEventContext>
			GetIncompleteMoveContexts(GameCard? card, Space? from, Space? to, IPlayer? player, IStackable? stackSrc)
		{
			if (card == null) return Enumerable.Empty<IIncompleteEventContext>();
			if (from == null) return Enumerable.Empty<IIncompleteEventContext>();
			if (to == null) return Enumerable.Empty<IIncompleteEventContext>();

			int distance = from.DistanceTo(to);

			var ret = new List<IIncompleteEventContext>();
			//Cards that from card is no longer in the AOE of
			var cardsMoverLeft = CardsAndAugsWhere(c => c != null && c.CardInAOE(card) && !c.SpaceInAOE(to));
			//Cards that from card no longer has in its aoe
			var cardsMoverLeftBehind = CardsAndAugsWhere(c => c != null && card.CardInAOE(c) && !card.CardInAOE(c, to));

			//Add contexts for 
			var baseContext = IEventContext.Build(Trigger.Anything)
				.CausedBy(stackSrc)
				.At(to)
				.ForPlayer(player)
				.WithX(distance);

			ret.AddRange(EnumerateMoveContexts(baseContext, card, cardsMoverLeft, cardsMoverLeftBehind));

            //trigger for first card's augments
            foreach (var aug in card.Augments)
			{
				ret.AddRange(EnumerateMoveContexts(baseContext, aug, cardsMoverLeft, cardsMoverLeftBehind));
			}
			return ret;
		}

		private static IEnumerable<IIncompleteEventContext> EnumerateMoveContexts(EventContextBuilder baseBuilder, GameCard mover,
			IEnumerable<GameCard> cardsMoverLeft, IEnumerable<GameCard> cardsMoverLeftBehind)
		{	
			var moveMover = baseBuilder.CloneForEvent(Trigger.Move)
				.PrimarilyAffecting(mover);
			var arriveMover = moveMover.CloneForEvent(Trigger.Arrive);

			//Cards that from card is no longer in the AOE of
			var baseLeaving = baseBuilder.CloneForEvent(Trigger.LeaveAOE)
				.PrimarilyAffecting(mover);
            var leaving = cardsMoverLeft.Select(moverLeft => baseLeaving.Clone().SecondarilyAffecting(moverLeft));

            //Cards that from card no longer has in its aoe
            var baseLeftBehind = baseBuilder.CloneForEvent(Trigger.LeaveAOE)
				.SecondarilyAffecting(mover);
			var leftBehind = cardsMoverLeftBehind.Select(moverLeftBehind => baseLeftBehind.Clone().PrimarilyAffecting(moverLeftBehind));

			return moveMover.Yield()
				.Concat(arriveMover.Yield())
				.Concat(leaving)
				.Concat(leftBehind);
		}
	}
}