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
	public class ServerNotifier
	{
		//I don't like doing this with a dict, but the other option is casting. I can decide which is the worse of two evils later, for now I wanna get it working
		private readonly Dictionary<IPlayer, ServerNetworker> networkerByPlayer = new();

		public ServerNotifier(ServerPlayer[] players)
		{
			foreach (var player in players) networkerByPlayer[player] = player.Networker;
		}

		public void SendPacket(IPlayer player, Packet packet)
		{
			//if (packet != null) GD.Print($"Sending packet to {player.Index} with info {packet}");
			networkerByPlayer[player].SendPacket(packet);
		}

		private void SendPackets(IPlayer playerA, Packet a, IPlayer playerB, Packet b)
		{
			SendPacket(playerA, a);
			SendPacket(playerB, b);
		}

		private void SendToAll(Packet p)
		{
			foreach(var (_, networker) in networkerByPlayer) networker.SendPacket(p.Copy());
		}

		private void SendToBothInverting (IPlayer originator, Packet p, bool known = true)
		{
			foreach (var (player, networker) in networkerByPlayer)
			{
				networker.SendPacket(originator == player ? p : p.GetInversion(known));
			}
		}

		#region game start
		public void GetDecklist(IPlayer player) => SendPacket(player, new GetDeckPacket());

		public void DeckAccepted(IPlayer player) => SendPacket(player, new DeckAcceptedPacket());

		public void SetFriendlyAvatar(IPlayer player, string json, int cardID)
			=> SendToBothInverting(player, new SetAvatarPacket(0, json, cardID));

		/// <summary>
		/// Takes care of inverting first turn player
		/// </summary>
		/// <param name="firstPlayer">First turn player, from the server's perspective</param>
		public void SetFirstTurnPlayer(IPlayer firstTurnPlayer)
		{
			SendToBothInverting(firstTurnPlayer, new SetFirstPlayerPacket(0));
		}
		#endregion game start

		public void NotifyWin(IPlayer player) => SendToBothInverting(player, new GameEndPacket(true));

		public void NotifyPutBack(IPlayer player) => SendPacket(player, new PutCardsBackPacket());

		public void NotifyBothPutBack() => SendToAll(new PutCardsBackPacket());

		#region game stats
		public void NotifyLeyload(int leyload) => SendToAll(new SetLeyloadPacket(leyload));

		public void NotifySetPips(IPlayer player, int pipsToSet)
			=> SendToBothInverting(player, new SetPipsPacket(pipsToSet, player.Index, invert: player.Index != 0));
		public void NotifyYourTurn(IPlayer player) => SendToBothInverting(player, new SetTurnPlayerPacket(0));

		public void NotifyDeckCount(IPlayer player, int count) => SendToBothInverting(player, new SetDeckCountPacket(0, count));
		#endregion game stats

		#region card location
		public void NotifyAttach(IPlayer player, GameCard toAttach, Space space, bool wasKnown)
			=> SendToBothInverting(player, new AttachCardPacket(toAttach, space.x, space.y, invert: player.Index != 0), wasKnown);

		/// <summary>
		/// Notifies that the IPlayer corresponding to this notifier played a given card
		/// </summary>
		public void NotifyPlay(IPlayer player, GameCard toPlay, Space space, bool wasKnown)
		{
			//if this card is an augment, don't bother notifying about it. attach will take care of it.
			if (toPlay.CardType == 'A') return;

			//tell everyone to do it
			var p = new PlayCardPacket(toPlay.ID, toPlay.BaseJson, toPlay.ControllerIndex, space.x, space.y, invert: player.Index != 0);
			var q = p.GetInversion(wasKnown);
			SendPackets(player, p, player.Enemy, q);
		}

		public void NotifyMove(IPlayer player, GameCard toMove, Space space)
			=> SendToBothInverting(player, new MoveCardPacket(toMove.ID, space.x, space.y, invert: player.Index != 0));

		public void NotifyDiscard(IPlayer player, GameCard toDiscard, bool wasKnown)
			=> SendToBothInverting(player, new DiscardCardPacket(toDiscard, invert: player.Index != 0), wasKnown);

		public void NotifyRehand(IPlayer player, GameCard toRehand, bool wasKnown)
			=> SendToBothInverting(player, new RehandCardPacket(toRehand.ID), wasKnown);

		public void NotifyDecrementHand(IPlayer player) => SendPacket(player, new ChangeEnemyHandCountPacket(-1));

		public void NotifyAnnhilate(IPlayer player, GameCard toAnnhilate, bool wasKnown)
			=> SendToBothInverting(player, new AnnihilateCardPacket(toAnnhilate.ID, toAnnhilate.BaseJson, toAnnhilate.ControllerIndex, invert: player.Index != 0),
				known: wasKnown);

		public void NotifyTopdeck(IPlayer player, GameCard card, bool wasKnown)
			=> SendToBothInverting(player, new TopdeckCardPacket(card.ID, card.OwnerIndex, invert: player.Index != 0),
				known: wasKnown);

		public void NotifyBottomdeck(IPlayer player, GameCard card, bool wasKnown)
			=> SendToBothInverting(player, new BottomdeckCardPacket(card.ID, card.OwnerIndex, invert: player.Index != 0),
				known: wasKnown);

		public void NotifyReshuffle(IPlayer player, GameCard toReshuffle, bool wasKnown)
			=> SendToBothInverting(player, new ReshuffleCardPacket(toReshuffle.ID, toReshuffle.OwnerIndex, invert: player.Index != 0),
				known: wasKnown);

		public void NotifyCreateCard(IPlayer player, GameCard added, bool wasKnown)
			=> SendToBothInverting(player, new AddCardPacket(added, invert: player.Index != 0), known: wasKnown);

		public void NotifyRevealCard(IPlayer player, GameCard revealed)
		{
			NotifyDecrementHand(player);
			SendPacket(player, new AddCardPacket(revealed, invert: player.Index != 0));
		}

		public void NotifyKnownToEnemy(IPlayer player, GameCard toUpdate, bool wasKnown)
			=> SendToBothInverting(player, new UpdateKnownToEnemyPacket(toUpdate.KnownToEnemy, toUpdate.ID), known: wasKnown);

		public void GetHandSizeChoices(IPlayer player, int[] cardIds, string listRestrictionJson)
			=> SendPacket(player, new GetHandSizeChoicesOrderPacket(cardIds, listRestrictionJson));

		public void NotifyHandSizeToStack(IPlayer player) => SendToBothInverting(player, new HandSizeToStackPacket(0));
		#endregion card location

		#region card stats
		public void NotifyStats(IPlayer player, GameCard card)
			=> SendToBothInverting(player, new ChangeCardNumericStatsPacket(card.ID, card.Stats, card.SpacesMoved), card.KnownToEnemy);

		public void NotifySpacesMoved(IPlayer player, GameCard card)
			=> SendToBothInverting(player, new SpacesMovedPacket(card.ID, card.SpacesMoved), card.KnownToEnemy);

		public void NotifyAttacksThisTurn(IPlayer player, GameCard card)
		{
			GD.Print("Notifying about attacks this turn...");
			SendToBothInverting(player, new AttacksThisTurnPacket(card.ID, card.AttacksThisTurn), card.KnownToEnemy);
		}

		public void NotifySetNegated(IPlayer player, GameCard card, bool negated)
			=> SendToBothInverting(player, new NegateCardPacket(card.ID, negated), card.KnownToEnemy);

		public void NotifyActivate(IPlayer player, GameCard card, bool activated)
			=> SendToBothInverting(player, new ActivateCardPacket(card.ID, activated), card.KnownToEnemy);

		public void NotifyChangeController(IPlayer player, GameCard card, IPlayer controller)
			=> SendToBothInverting(player, new ChangeCardControllerPacket(card.ID, controller.Index, invert: player.Index != 0), card.KnownToEnemy);
		#endregion card stats

		#region request targets
		public void GetCardTarget(IPlayer player, string cardName, string targetBlurb, int[] ids, string listRestrictionJson, bool list)
			=> SendPacket(player, new GetCardTargetPacket(cardName, targetBlurb, ids, listRestrictionJson, list));

		public void GetSpaceTarget(IPlayer player, string cardName, string targetBlurb, (int, int)[] spaces, (int, int)[] recommendedSpaces)
			=> SendPacket(player, new GetSpaceTargetPacket(cardName, targetBlurb, spaces, recommendedSpaces));
		#endregion request targets

		public void NotifyAttackStarted(IPlayer player, GameCard atk, GameCard def, IPlayer initiator)
			=> SendToAll(new AttackStartedPacket(atk.ID, def.ID, initiator.Index));

		#region other effect stuff
		public void ChooseEffectOption(IPlayer player, string cardName, string choiceBlurb, string[] optionBlurbs, bool hasDefault, bool showX, int x)
			=> SendPacket(player, new GetEffectOptionPacket(cardName, choiceBlurb, optionBlurbs, hasDefault, showX, x));

		public void EffectResolving(IPlayer player, ServerEffect eff)
			=> SendToBothInverting(player, new EffectResolvingPacket(eff.Card.ID, eff.EffectIndex, player.Index, invert: player.Index != 0));

		public void NotifyEffectActivated(IPlayer player, ServerEffect eff)
			=> SendToAll(new EffectActivatedPacket(eff.Card.ID, eff.EffectIndex));

		public void RemoveStackEntry(int i) => SendToAll(new RemoveStackEntryPacket(i));

		public void EffectImpossible() => SendToAll(new EffectImpossiblePacket());

		public void RequestResponse(IPlayer player) => SendPacket(player, new ToggleAllowResponsesPacket(true));

		public void RequestNoResponse(IPlayer player) => SendPacket(player, new ToggleAllowResponsesPacket(false));

		/// <summary>
		/// Lets that player know their target has been accepted. called if the Target method returns True
		/// </summary>
		public void AcceptTarget(IPlayer player) => SendPacket(player, new TargetAcceptedPacket());

		public void StackEmpty() => SendToAll(new StackEmptyPacket());

		public void AddTarget(GameCard source, int effIndex, GameCard target)
			=> SendToAll(new AddTargetPacket(source.ID, effIndex, target.ID));

		public void AddHiddenTarget(IPlayer player, GameCard source, int effIndex, GameCard target)
			=> SendPacket(player, new AddTargetPacket(source.ID, effIndex, target.ID));

		public void RemoveTarget(GameCard source, int effIndex, GameCard target)
			=> SendToAll(new RemoveTargetPacket(source.ID, effIndex, target.ID));

		public void GetXForEffect(IPlayer player) => SendPacket(player, new GetPlayerChooseXPacket());

		public void NotifyEffectX(GameCard effSrc, int effIndex, int x)
		{
			var p = new SetEffectsXPacket(effSrc.ID, effIndex, x);
			SendToAll(p);
		}

		public void EnableDecliningTarget(IPlayer player) => SendPacket(player, new ToggleDecliningTargetPacket(true));

		public void DisableDecliningTarget(IPlayer player) => SendPacket(player, new ToggleDecliningTargetPacket(false));

		public void AskForTrigger(IPlayer player, ServerTrigger t, int x, bool showX)
			=> SendToBothInverting(player, new OptionalTriggerPacket(t.Effect.Card.ID, t.Effect.EffectIndex, x, showX));

		public void GetTriggerOrder(IPlayer player, IEnumerable<ServerTrigger> triggers)
		{
			int[] cardIds = triggers.Select(t => t.Effect.Card.ID).ToArray();
			int[] effIndices = triggers.Select(t => t.Effect.EffectIndex).ToArray();
			SendPacket(player, new GetTriggerOrderPacket(cardIds, effIndices));
		}

		public void AddCardLink(CardLink link) => SendToAll(new EditCardLinkPacket(link, add: true));
		public void AddHiddenCardLink(IPlayer player, CardLink link) => SendPacket(player, new EditCardLinkPacket(link, add: true));
		public void RemoveCardLink(CardLink link) => SendToAll(new EditCardLinkPacket(link, add: false));
		#endregion other effect stuff
	}
}