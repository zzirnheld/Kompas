
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
	public class ClientEffect : Effect, IClientStackable
	{
		private IPlayer owningPlayer;

		public override IPlayer OwningPlayer => owningPlayer;

		public ClientGame ClientGame { get; private set; }
		public override IGame Game => ClientGame;
		public DummySubeffect[] DummySubeffects { get; }
		public ClientTrigger ClientTrigger { get; private set; }

		public override Subeffect[] Subeffects => DummySubeffects;
		public override Trigger Trigger => ClientTrigger;

		private IResolutionContext currentResolutionContext;
		public override IResolutionContext CurrentResolutionContext
			=> currentResolutionContext ??= ResolutionContext.PlayerTrigger(this, Game);
		//TODO controller? should have some way to track it client-side otherwise if effects ever can be activated by not the card's ocntroller something will break

		public string StackableBlurb => blurb;

		private ClientGameCard card;
		public override GameCard Card => card;

		public void SetInfo(ClientGameCard card, ClientGame clientGame, int effectIndex, IPlayer owningPlayer)
		{
			this.card = card;
			ClientGame = clientGame;
			this.owningPlayer = owningPlayer;
			base.SetInfo(effectIndex);
			if (triggerData != null && !string.IsNullOrEmpty(triggerData.triggerCondition))
				ClientTrigger = new ClientTrigger(triggerData, this);
		}

		public override void AddTarget(GameCard card)
		{
			base.AddTarget(card);
			//card.CardController.gameCardViewController.Refresh();
		}

		public override void RemoveTarget(GameCard card)
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