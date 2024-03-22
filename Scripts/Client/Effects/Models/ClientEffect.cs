
using System;
using Kompas.Cards.Models;
using Kompas.Client.Cards.Models;
using Kompas.Client.Gamestate;
using Kompas.Client.Gamestate.Players;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Effects.Models
{
	//TODO refactor into serialized - unserialized thing? Or reformat like Identities?
	public class ClientEffect : Effect, IClientStackable
	{
		private IPlayer? owningPlayer;
		public override IPlayer OwningPlayer => owningPlayer ?? throw new System.NullReferenceException("Tried to get owning player of uninitialized effect");

		private ClientGameCard? card;
		public override IGameCard Card => card ?? throw new System.NullReferenceException("Tried to get card of uninitialized effect");

		private ClientGame? _clientGame;
		public ClientGame ClientGame
		{
			get => _clientGame ?? throw new System.NullReferenceException("Tried to get game of uninitialized effect");
			private set => _clientGame = value;
		}
		private ClientTrigger? _clientTrigger;
		public ClientTrigger ClientTrigger
		{
			get => _clientTrigger ?? throw new System.NullReferenceException("Tried to get trigger of uninitialized effect");
			private set => _clientTrigger = value;
		}
		public override IGame Game => ClientGame;

		public DummySubeffect[] DummySubeffects { get; } = Array.Empty<DummySubeffect>();
		public override Subeffect[] Subeffects => DummySubeffects;
		public override Trigger Trigger => ClientTrigger;

		private IResolutionContext? currentResolutionContext;
		public override IResolutionContext CurrentResolutionContext
			=> currentResolutionContext ??= ResolutionContext.PlayerTrigger(this, Game);
		//TODO controller? should have some way to track it client-side otherwise if effects ever can be activated by not the card's ocntroller something will break

		public string StackableBlurb => blurb ?? string.Empty;

		public void SetInfo(ClientGameCard card, ClientGame clientGame, int effectIndex, IPlayer owningPlayer)
		{
			this.card = card;
			ClientGame = clientGame;
			this.owningPlayer = owningPlayer;
			base.SetInfo(effectIndex);
			if (triggerData != null && !string.IsNullOrEmpty(triggerData.triggerCondition))
				ClientTrigger = new ClientTrigger(triggerData, this);
		}

		public override void AddTarget(IGameCard card)
		{
			base.AddTarget(card);
			//card.CardController.gameCardViewController.Refresh();
		}

		public override void RemoveTarget(IGameCard card)
		{
			base.RemoveTarget(card);
			//card.CardController.gameCardViewController.Refresh();
		}

		//TODO eventually make client aware of activation contexts
		public void IncrementUses()
		{
			TimesUsedThisTurn++;
			TimesUsedThisRound++;
			TimesUsedThisStack++;
		}

		public void ResolutionStarted()
		{
			CardTargets.Clear();
		}
	}
}