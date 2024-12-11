using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Networking;

namespace Kompas.Server.Gamestate.Locations.Models;

public class ServerDeck : Deck
{
	private readonly ServerGame game;

	public ServerDeck(IPlayer owner, DeckController deckController, ServerGame game)
		: base(owner, deckController)
	{
		this.game = game;
	}

	protected override void PerformAdd(GameCard card, int? index = null, IStackable? stackSrc = null)
	{
		var contexts = IEventContext.Build(Trigger.ToDeck)
			.PrimarilyAffecting(card)
			.CausedBy(stackSrc)
			.ForPlayer(Owner)
			.Capture(() => base.PerformAdd(card, index, stackSrc));
		game.StackController.TriggerFor(contexts);

		ServerNotifier.NotifyDeckCount(Owner, Cards.Count());
	}

	public override void PushBottomdeck(GameCard card, IStackable? stackSrc = null)
	{
		bool wasKnown = card.KnownToEnemy;

		var contexts = IEventContext.Build(Trigger.Bottomdeck)
			.PrimarilyAffecting(card)
			.CausedBy(stackSrc)
			.ForPlayer(Owner)
			.Capture(() => base.PushBottomdeck(card, stackSrc));
		game.StackController.TriggerFor(contexts);

		ServerNotifier.NotifyBottomdeck(Owner, card, wasKnown);
	}

	public override void PushTopdeck(GameCard card, IStackable? stackSrc = null)
	{
		bool wasKnown = card.KnownToEnemy;

		var contexts = IEventContext.Build(Trigger.Topdeck)
			.PrimarilyAffecting(card)
			.CausedBy(stackSrc)
			.ForPlayer(Owner)
			.Capture(() => base.PushTopdeck(card, stackSrc));
		game.StackController.TriggerFor(contexts);

		ServerNotifier.NotifyTopdeck(Owner, card, wasKnown);
	}

	public override void ShuffleIn(GameCard card, IStackable? stackSrc = null)
	{
		bool wasKnown = card.KnownToEnemy;

		var contexts = IEventContext.Build(Trigger.Reshuffle)
			.PrimarilyAffecting(card)
			.CausedBy(stackSrc)
			.ForPlayer(Owner)
			.Capture(() => base.ShuffleIn(card, stackSrc));
		game.StackController.TriggerFor(contexts);

		ServerNotifier.NotifyReshuffle(Owner, card, wasKnown);
	}

	public override void Remove(GameCard card)
	{
		base.Remove(card);
		Networking.ServerNotifier.NotifyDeckCount(Owner, Cards.Count());
	}
}