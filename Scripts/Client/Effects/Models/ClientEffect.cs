
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
	public class ClientEffect : Effect<ClientGameCard, ClientPlayer>, IClientStackable
	{
		private ClientPlayer? _owningPlayer;
		public override ClientPlayer OwningPlayer => _owningPlayer
			?? throw new System.NullReferenceException("Tried to get owning player of uninitialized effect");

		private ClientGameCard? _card;
		public override ClientGameCard Card => _card
			?? throw new System.NullReferenceException("Tried to get card of uninitialized effect");

		private ClientGame? _clientGame;
		public ClientGame ClientGame
		{
			get => _clientGame ?? throw new System.NullReferenceException("Tried to get game of uninitialized effect");
			private set => _clientGame = value;
		}
		public override IGame<ClientGameCard, ClientPlayer> Game => ClientGame;

		private ClientTrigger? _clientTrigger;
		public ClientTrigger ClientTrigger
		{
			get => _clientTrigger ?? throw new System.NullReferenceException("Tried to get trigger of uninitialized effect");
			private set => _clientTrigger = value;
		}

		public DummySubeffect[] DummySubeffects { get; } = Array.Empty<DummySubeffect>();
		public override Subeffect[] Subeffects => DummySubeffects;
		public override Trigger Trigger => ClientTrigger;

		private IResolutionContext<ClientGameCard, ClientPlayer>? _currentResolutionContext;
		public override IResolutionContext<ClientGameCard, ClientPlayer> CurrentResolutionContext
			=> _currentResolutionContext ??= IResolutionContext.PlayerTrigger(this, Game);
		//TODO controller? should have some way to track it client-side otherwise if effects ever can be activated by not the card's ocntroller something will break

		public string StackableBlurb => blurb ?? string.Empty;

		public void SetInfo(ClientGameCard card, ClientGame clientGame, int effectIndex, ClientPlayer owningPlayer)
		{
			this._card = card;
			ClientGame = clientGame;
			this._owningPlayer = owningPlayer;
			base.SetInfo(effectIndex);
			if (triggerData != null && !string.IsNullOrEmpty(triggerData.triggerCondition))
				ClientTrigger = new ClientTrigger(triggerData, this);
		}

		public override void AddTarget(ClientGameCard card)
		{
			base.AddTarget(card);
			//card.CardController.gameCardViewController.Refresh();
		}

		public override void RemoveTarget(ClientGameCard card)
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