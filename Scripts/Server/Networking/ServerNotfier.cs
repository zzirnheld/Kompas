using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;
using Kompas.Networking;
using Kompas.Networking.Packets;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate.Players;

namespace Kompas.Server.Networking
{
	//TODO refactor all of these functions.
	//it wasn't broke for a while so I didn't fix it, but then it broke when I changed to godot so now i can fix it
	public static class ServerNotifier
	{
		public static void SendPacket(IPlayer player, Packet packet)
		{
			if (packet != null) GD.Print($"Sending packet to {player.Index} with info {packet}");
			player.Networker.SendPacket(packet);
		}

		private static void SendPackets(IPlayer playerA, Packet a, IPlayer playerB, Packet b)
		{
			SendPacket(playerA, a);
			SendPacket(playerB, b);
		}

		private static void SendToAll(Packet p, IPlayer[] players)
		{
			foreach(var player in players) player.Networker.SendPacket(p.Copy());
		}

		private static void SendToBothInverting (IPlayer originator, Packet p, bool known = true)
		{
			originator.Networker.SendPacket(p);
			originator.Enemy.Networker.SendPacket(p.GetInversion(known));
		}

		#region game start
		public static void GetDecklist(IPlayer player) => SendPacket(player, new GetDeckPacket());

		public static void DeckAccepted(IPlayer player) => SendPacket(player, new DeckAcceptedPacket());

		public static void SetFriendlyAvatar(IPlayer player, string json, int cardID)
			=> SendToBothInverting(player, new SetAvatarPacket(0, json, cardID));

		/// <summary>
		/// Takes care of inverting first turn player
		/// </summary>
		/// <param name="firstPlayer">First turn player, from the server's perspective</param>
		public static void SetFirstTurnPlayer(IPlayer firstTurnPlayer)
		{
			SendToBothInverting(firstTurnPlayer, new SetFirstPlayerPacket(0));
		}
		#endregion game start

		public static void NotifyWin(IPlayer player) => SendToBothInverting(player, new GameEndPacket(true));

		public static void NotifyPutBack(IPlayer player) => SendPacket(player, new PutCardsBackPacket());

		public static void NotifyBothPutBack(IPlayer[] players) => SendToAll(new PutCardsBackPacket(), players);

		#region game stats
		public static void NotifyLeyload(int leyload, IPlayer[] players) => SendToAll(new SetLeyloadPacket(leyload), players);

		public static void NotifySetPips(IPlayer player, int pipsToSet)
			=> SendToBothInverting(player, new SetPipsPacket(pipsToSet, player.Index, invert: player.Index != 0));
		public static void NotifyYourTurn(IPlayer player) => SendToBothInverting(player, new SetTurnPlayerPacket(0));

		public static void NotifyDeckCount(IPlayer player, int count) => SendToBothInverting(player, new SetDeckCountPacket(0, count));
		#endregion game stats

		#region card location
		public static void NotifyAttach(IPlayer player, GameCard toAttach, Space space, bool wasKnown)
			=> SendToBothInverting(player, new AttachCardPacket(toAttach, space.x, space.y, invert: player.Index != 0), wasKnown);

		/// <summary>
		/// Notifies that the IPlayer corresponding to this notifier played a given card
		/// </summary>
		public static void NotifyPlay(IPlayer player, GameCard toPlay, Space space, bool wasKnown)
		{
			//if this card is an augment, don't bother notifying about it. attach will take care of it.
			if (toPlay.CardType == 'A') return;

			//tell everyone to do it
			var p = new PlayCardPacket(toPlay.ID, toPlay.BaseJson, toPlay.ControllerIndex, space.x, space.y, invert: player.Index != 0);
			var q = p.GetInversion(wasKnown);
			SendPackets(player, p, player.Enemy, q);
		}

		public static void NotifyMove(IPlayer player, GameCard toMove, Space space)
			=> SendToBothInverting(player, new MoveCardPacket(toMove.ID, space.x, space.y, invert: player.Index != 0));

		public static void NotifyDiscard(IPlayer player, GameCard toDiscard, bool wasKnown)
			=> SendToBothInverting(player, new DiscardCardPacket(toDiscard, invert: player.Index != 0), wasKnown);

		public static void NotifyRehand(IPlayer player, GameCard toRehand, bool wasKnown)
			=> SendToBothInverting(player, new RehandCardPacket(toRehand.ID), wasKnown);

		public static void NotifyDecrementHand(IPlayer player) => SendPacket(player, new ChangeEnemyHandCountPacket(-1));

		public static void NotifyAnnhilate(IPlayer player, GameCard toAnnhilate, bool wasKnown)
			=> SendToBothInverting(player, new AnnihilateCardPacket(toAnnhilate.ID, toAnnhilate.BaseJson, toAnnhilate.ControllerIndex, invert: player.Index != 0),
				known: wasKnown);

		public static void NotifyTopdeck(IPlayer player, GameCard card, bool wasKnown)
			=> SendToBothInverting(player, new TopdeckCardPacket(card.ID, card.OwnerIndex, invert: player.Index != 0),
				known: wasKnown);

		public static void NotifyBottomdeck(IPlayer player, GameCard card, bool wasKnown)
			=> SendToBothInverting(player, new BottomdeckCardPacket(card.ID, card.OwnerIndex, invert: player.Index != 0),
				known: wasKnown);

		public static void NotifyReshuffle(IPlayer player, GameCard toReshuffle, bool wasKnown)
			=> SendToBothInverting(player, new ReshuffleCardPacket(toReshuffle.ID, toReshuffle.OwnerIndex, invert: player.Index != 0),
				known: wasKnown);

		public static void NotifyCreateCard(IPlayer player, GameCard added, bool wasKnown)
			=> SendToBothInverting(player, new AddCardPacket(added, invert: player.Index != 0), known: wasKnown);

		public static void NotifyRevealCard(IPlayer player, GameCard revealed)
		{
			NotifyDecrementHand(player);
			SendPacket(player, new AddCardPacket(revealed, invert: player.Index != 0));
		}

		public static void NotifyKnownToEnemy(IPlayer player, GameCard toUpdate, bool wasKnown)
			=> SendToBothInverting(player, new UpdateKnownToEnemyPacket(toUpdate.KnownToEnemy, toUpdate.ID), known: wasKnown);

		public static void GetHandSizeChoices(IPlayer player, int[] cardIds, string listRestrictionJson)
			=> SendPacket(player, new GetHandSizeChoicesOrderPacket(cardIds, listRestrictionJson));

		public static void NotifyHandSizeToStack(IPlayer player) => SendToBothInverting(player, new HandSizeToStackPacket(0));
		#endregion card location

		#region card stats
		public static void NotifyStats(IPlayer player, GameCard card)
			=> SendToBothInverting(player, new ChangeCardNumericStatsPacket(card.ID, card.Stats, card.SpacesMoved), card.KnownToEnemy);

		public static void NotifySpacesMoved(IPlayer player, GameCard card)
			=> SendToBothInverting(player, new SpacesMovedPacket(card.ID, card.SpacesMoved), card.KnownToEnemy);

		public static void NotifyAttacksThisTurn(IPlayer player, GameCard card)
		{
			GD.Print("Notifying about attacks this turn...");
			SendToBothInverting(player, new AttacksThisTurnPacket(card.ID, card.AttacksThisTurn), card.KnownToEnemy);
		}

		public static void NotifySetNegated(IPlayer player, GameCard card, bool negated)
			=> SendToBothInverting(player, new NegateCardPacket(card.ID, negated), card.KnownToEnemy);

		public static void NotifyActivate(IPlayer player, GameCard card, bool activated)
			=> SendToBothInverting(player, new ActivateCardPacket(card.ID, activated), card.KnownToEnemy);

		public static void NotifyChangeController(IPlayer player, GameCard card, IPlayer controller)
			=> SendToBothInverting(player, new ChangeCardControllerPacket(card.ID, controller.Index, invert: player.Index != 0), card.KnownToEnemy);
		#endregion card stats

		#region request targets
		public static void GetCardTarget(IPlayer player, string cardName, string targetBlurb, int[] ids, string listRestrictionJson, bool list)
			=> SendPacket(player, new GetCardTargetPacket(cardName, targetBlurb, ids, listRestrictionJson, list));

		public static void GetSpaceTarget(IPlayer player, string cardName, string targetBlurb, (int, int)[] spaces, (int, int)[] recommendedSpaces)
			=> SendPacket(player, new GetSpaceTargetPacket(cardName, targetBlurb, spaces, recommendedSpaces));
		#endregion request targets

		public static void NotifyAttackStarted(IPlayer instigator, GameCard atk, GameCard def)
			=> SendToAll(new AttackStartedPacket(atk.ID, def.ID, instigator.Index), new IPlayer[] {instigator, instigator.Enemy});

		#region other effect stuff
		public static void ChooseEffectOption(IPlayer player, string cardName, string choiceBlurb, string[] optionBlurbs, bool hasDefault, bool showX, int x)
			=> SendPacket(player, new GetEffectOptionPacket(cardName, choiceBlurb, optionBlurbs, hasDefault, showX, x));

		public static void EffectResolving(IPlayer player, ServerEffect eff)
			=> SendToBothInverting(player, new EffectResolvingPacket(eff.Card.ID, eff.EffectIndex, player.Index, invert: player.Index != 0));

		public static void NotifyEffectActivated(IPlayer player, ServerEffect eff)
			=> SendToAll(new EffectActivatedPacket(eff.Card.ID, eff.EffectIndex), new IPlayer[] {player, player.Enemy});

		public static void RemoveStackEntry(int i, IPlayer[] players) => SendToAll(new RemoveStackEntryPacket(i), players);

		public static void EffectImpossible(IPlayer[] players) => SendToAll(new EffectImpossiblePacket(), players);

		public static void RequestResponse(IPlayer player) => SendPacket(player, new ToggleAllowResponsesPacket(true));

		public static void RequestNoResponse(IPlayer player) => SendPacket(player, new ToggleAllowResponsesPacket(false));

		/// <summary>
		/// Lets that player know their target has been accepted. called if the Target method returns True
		/// </summary>
		public static void AcceptTarget(IPlayer player) => SendPacket(player, new TargetAcceptedPacket());

		public static void StackEmpty(IPlayer[] players) => SendToAll(new StackEmptyPacket(), players);

		public static void AddTarget(GameCard source, int effIndex, GameCard target, IPlayer[] players)
			=> SendToAll(new AddTargetPacket(source.ID, effIndex, target.ID), players);

		public static void AddHiddenTarget(IPlayer player, GameCard source, int effIndex, GameCard target)
			=> SendPacket(player, new AddTargetPacket(source.ID, effIndex, target.ID));

		public static void RemoveTarget(GameCard source, int effIndex, GameCard target, IPlayer[] players)
			=> SendToAll(new RemoveTargetPacket(source.ID, effIndex, target.ID), players);

		public static void GetXForEffect(IPlayer player) => SendPacket(player, new GetPlayerChooseXPacket());

		public static void NotifyEffectX(GameCard effSrc, int effIndex, int x, IPlayer[] players)
		{
			var p = new SetEffectsXPacket(effSrc.ID, effIndex, x);
			SendToAll(p, players);
		}

		public static void EnableDecliningTarget(IPlayer player) => SendPacket(player, new ToggleDecliningTargetPacket(true));

		public static void DisableDecliningTarget(IPlayer player) => SendPacket(player, new ToggleDecliningTargetPacket(false));

		public static void AskForTrigger(IPlayer player, ServerTrigger t, int x, bool showX)
			=> SendToBothInverting(player, new OptionalTriggerPacket(t.Effect.Card.ID, t.Effect.EffectIndex, x, showX));

		public static void GetTriggerOrder(IPlayer player, IEnumerable<ServerTrigger> triggers)
		{
			int[] cardIds = triggers.Select(t => t.Effect.Card.ID).ToArray();
			int[] effIndices = triggers.Select(t => t.Effect.EffectIndex).ToArray();
			SendPacket(player, new GetTriggerOrderPacket(cardIds, effIndices));
		}

		public static void AddCardLink(CardLink link, IPlayer[] players) => SendToAll(new EditCardLinkPacket(link, add: true), players);
		public static void AddHiddenCardLink(IPlayer player, CardLink link) => SendPacket(player, new EditCardLinkPacket(link, add: true));
		public static void RemoveCardLink(CardLink link, IPlayer[] players) => SendToAll(new EditCardLinkPacket(link, add: false), players);
		#endregion other effect stuff
	}
}