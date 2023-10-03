
using Kompas.Cards.Models;
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
		private ClientPlayer controllingPlayer;
		public override Player ControllingPlayer => controllingPlayer;

		public ClientGame ClientGame { get; private set; }
		public override Game Game => ClientGame;
		public DummySubeffect[] DummySubeffects { get; }
		public ClientTrigger ClientTrigger { get; private set; }

		public override Subeffect[] Subeffects => DummySubeffects;
		public override Trigger Trigger => ClientTrigger;

		public override IResolutionContext CurrentResolutionContext
		{
			get
			{
				if (base.CurrentResolutionContext == null) CurrentResolutionContext = ResolutionContext.PlayerTrigger(this, Game);
				return base.CurrentResolutionContext;
			}
			protected set => base.CurrentResolutionContext = value;
		}

		public string StackableBlurb => blurb;

		public void SetInfo(GameCard thisCard, ClientGame clientGame, int effectIndex, ClientPlayer owner)
		{
			this.ClientGame = clientGame;
			controllingPlayer = owner;
			base.SetInfo(thisCard, effectIndex, owner);
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
		public void Activated(ResolutionContext context = default)
		{
			TimesUsedThisTurn++;
			TimesUsedThisRound++;
			TimesUsedThisStack++;

			ClientGame.EffectActivated(this);
			ClientGame.StackController.Add(this, context);
		}

		public void StartResolution(TriggeringEventContext context)
		{
			//TODO
			//ClientGame.clientUIController.SetCurrState($"Resolving Effect of {Source.CardName}", $"{blurb}");
			CardTargets.Clear();

			//in case any cards are still showing targets from the last effect, which they will if this happens after another effect in the stack.
			//TODO move this behavior to a "effect end" packet and stuff?
			ClientGame.ShowNoTargets();
		}
	}
}