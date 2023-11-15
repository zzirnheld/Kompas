using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Cards.Models;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Gamestate.Players;
using Kompas.Server.Networking;

namespace Kompas.Server.Gamestate.Locations.Models
{
	public class ServerBoard : Board
	{
		private readonly ServerGame serverGame;

		private ServerStackController EffectsController => serverGame.StackController;

		public ServerBoard(BoardController boardController, ServerGame serverGame) : base(boardController)
		{
			this.serverGame = serverGame;
		}

		public override void Play(GameCard toPlay, Space to, IPlayer controller, IStackable? stackSrc = null)
		{
			var context = new TriggeringEventContext(game: serverGame, CardBefore: toPlay, stackableCause: stackSrc, player: controller, space: to);
			bool wasKnown = toPlay.KnownToEnemy;
			base.Play(toPlay, to, controller, stackSrc: stackSrc);
			context.CacheCardInfoAfter();
			EffectsController.TriggerForCondition(Trigger.Play, context);
			EffectsController.TriggerForCondition(Trigger.Arrive, context);

			if (!toPlay.IsAvatar) ServerNotifier.NotifyPlay(controller, toPlay, to, wasKnown);
		}

		private (IEnumerable<TriggeringEventContext> moveContexts, IEnumerable<TriggeringEventContext> leaveContexts)
			GetContextsForMove(GameCard card, Space from, Space to, IPlayer player, IStackable? stackSrc)
		{
			int distance = from.DistanceTo(to);

			var moveContexts = new List<TriggeringEventContext>();
			var leaveContexts = new List<TriggeringEventContext>();
			//Cards that from card is no longer in the AOE of
			var cardsMoverLeft = CardsAndAugsWhere(c => c != null && c.CardInAOE(card) && !c.SpaceInAOE(to));
			//Cards that from card no longer has in its aoe
			var cardsMoverLeftBehind = CardsAndAugsWhere(c => c != null && card.CardInAOE(c) && !card.CardInAOE(c, to));

			//Add contexts for 
			moveContexts.Add(new TriggeringEventContext(game: serverGame, CardBefore: card, stackableCause: stackSrc, space: to,
				player: player, x: distance));
			//Cards that from card is no longer in the AOE of
			leaveContexts.AddRange(cardsMoverLeft.Select(c =>
				new TriggeringEventContext(game: serverGame, CardBefore: card, secondaryCardBefore: c, stackableCause: stackSrc, player: player)));
			//Cards that from card no longer has in its aoe
			leaveContexts.AddRange(cardsMoverLeftBehind.Select(c =>
				new TriggeringEventContext(game: serverGame, CardBefore: c, secondaryCardBefore: card, stackableCause: stackSrc, player: player)));
			//trigger for first card's augments
			foreach (var aug in card.Augments)
			{
				//Add contexts for 
				moveContexts.Add(new TriggeringEventContext(game: serverGame, CardBefore: aug, stackableCause: stackSrc, space: to,
					player: player, x: distance));
				//Cards that from aug is no longer in the AOE of
				leaveContexts.AddRange(cardsMoverLeft.Select(c =>
					new TriggeringEventContext(game: serverGame, CardBefore: aug, secondaryCardBefore: c, stackableCause: stackSrc, player: player)));
				//Cards that from aug no longer has in its aoe
				leaveContexts.AddRange(cardsMoverLeftBehind.Select(c =>
					new TriggeringEventContext(game: serverGame, CardBefore: c, secondaryCardBefore: aug, stackableCause: stackSrc, player: player)));
			}
			return (moveContexts, leaveContexts);
		}

		protected override void Swap(GameCard card, Space to, bool normal, IPlayer mover, IStackable? stackSrc = null)
		{
			//calculate distance before doing the swap
			var from = card.Position?.Copy;
			var at = GetCardAt(to);

			//then trigger appropriate triggers. list of contexts:
			var moveContexts = new List<TriggeringEventContext>();
			var leaveContexts = new List<TriggeringEventContext>();

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
				ctxt.CacheCardInfoAfter();
			}

			EffectsController.TriggerForCondition(Trigger.Move, moveContexts.ToArray());
			EffectsController.TriggerForCondition(Trigger.Arrive, moveContexts.ToArray());
			EffectsController.TriggerForCondition(Trigger.LeaveAOE, leaveContexts.ToArray());

			//notify the players
			ServerNotifier.NotifyMove(mover, card, to);
		}
	}
}