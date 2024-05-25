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
			var context = new TriggeringEventContext(game: serverGame, cardBefore: toPlay, stackableCause: stackSrc, player: controller, space: to);
			bool wasKnown = toPlay.KnownToEnemy;
			base.Play(toPlay, to, controller, stackSrc: stackSrc);
			context.CacheAfterEvent();
			EffectsController.TriggerForCondition(Trigger.Play, context);
			EffectsController.TriggerForCondition(Trigger.Arrive, context);

			if (!toPlay.IsAvatar) ServerNotifier.NotifyPlay(controller, toPlay, to, wasKnown);
		}

		private (IEnumerable<IEventContext> moveContexts, IEnumerable<IEventContext> leaveContexts)
			GetContextsForMove(GameCard card, Space from, Space to, IPlayer? player, IStackable? stackSrc)
		{
			int distance = from.DistanceTo(to);

			var moveContexts = new List<IEventContext>();
			var leaveContexts = new List<IEventContext>();
			//Cards that from card is no longer in the AOE of
			var cardsMoverLeft = CardsAndAugsWhere(c => c != null && c.CardInAOE(card) && !c.SpaceInAOE(to));
			//Cards that from card no longer has in its aoe
			var cardsMoverLeftBehind = CardsAndAugsWhere(c => c != null && card.CardInAOE(c) && !card.CardInAOE(c, to));

			//Add contexts for 
			moveContexts.Add(new TriggeringEventContext(game: serverGame, cardBefore: card, stackableCause: stackSrc, space: to,
				player: player, x: distance));
			//Cards that from card is no longer in the AOE of
			leaveContexts.AddRange(cardsMoverLeft.Select(c =>
				new TriggeringEventContext(game: serverGame, cardBefore: card, secondaryCardBefore: c, stackableCause: stackSrc, player: player)));
			//Cards that from card no longer has in its aoe
			leaveContexts.AddRange(cardsMoverLeftBehind.Select(c =>
				new TriggeringEventContext(game: serverGame, cardBefore: c, secondaryCardBefore: card, stackableCause: stackSrc, player: player)));
			//trigger for first card's augments
			foreach (var aug in card.Augments)
			{
				//Add contexts for 
				moveContexts.Add(new TriggeringEventContext(game: serverGame, cardBefore: aug, stackableCause: stackSrc, space: to,
					player: player, x: distance));
				//Cards that from aug is no longer in the AOE of
				leaveContexts.AddRange(cardsMoverLeft.Select(c =>
					new TriggeringEventContext(game: serverGame, cardBefore: aug, secondaryCardBefore: c, stackableCause: stackSrc, player: player)));
				//Cards that from aug no longer has in its aoe
				leaveContexts.AddRange(cardsMoverLeftBehind.Select(c =>
					new TriggeringEventContext(game: serverGame, cardBefore: c, secondaryCardBefore: aug, stackableCause: stackSrc, player: player)));
			}
			return (moveContexts, leaveContexts);
		}

		protected override void Swap(GameCard card, Space to, bool normal, IPlayer? mover = null, IStackable? stackSrc = null)
		{
			//calculate distance before doing the swap
			var from = card.Position?.Copy;
			var at = GetCardAt(to);

			//then trigger appropriate triggers. list of contexts:
			var moveContexts = new List<IEventContext>();
			var leaveContexts = new List<IEventContext>();

			if (from != null)
			{
				var (fromCardMoveContexts, fromCardLeaveContexts) = GetContextsForMove(card, from, to, mover, stackSrc);
				moveContexts.AddRange(fromCardMoveContexts);
				leaveContexts.AddRange(fromCardLeaveContexts);

				if (at != null)
				{
					var (atCardMoveContexts, atCardLeaveContexts) = GetContextsForMove(at, to, from, mover, stackSrc);
					moveContexts.AddRange(atCardMoveContexts);
					leaveContexts.AddRange(atCardLeaveContexts);
				}
			}

			//actually perform the swap
			base.Swap(card, to, normal, mover, stackSrc: stackSrc);

			foreach (var ctxt in moveContexts)
			{
				ctxt.CacheAfterEvent();
			}

			EffectsController.TriggerForCondition(Trigger.Move, moveContexts.ToArray());
			EffectsController.TriggerForCondition(Trigger.Arrive, moveContexts.ToArray());
			EffectsController.TriggerForCondition(Trigger.LeaveAOE, leaveContexts.ToArray());

			var contexts = EventCapturer.Capture(GetIncompleteMoveContexts(card, from, to, mover, stackSrc),
				() => base.Swap(card, to, normal, mover, stackSrc: stackSrc));

			foreach (var context in contexts) EffectsController.TriggerForCondition(context.TriggeringEvent, context);
			
			//notify the players
			ServerNotifier.NotifyMove(mover ?? card.OwningPlayer, card, to);
		}

		private IEnumerable<IIncompleteEventContext>
			GetIncompleteMoveContexts(GameCard card, Space? from, Space to, IPlayer? player, IStackable? stackSrc)
		{
			if (from == null) return Enumerable.Empty<IIncompleteEventContext>();

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

			return 		EnumerableHelper.Yield(moveMover)
				.Concat(EnumerableHelper.Yield(arriveMover))
				.Concat(leaving)
				.Concat(leftBehind);
		}
	}
}