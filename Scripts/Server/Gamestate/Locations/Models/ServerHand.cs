using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Controllers;

namespace Kompas.Server.Gamestate.Locations.Models;

public class ServerHand : Hand
{
	private readonly ServerGame game;

	public ServerHand(IPlayer owner, IHandController handController, ServerGame game)
		: base(owner, handController)
	{
		this.game = game;
	}

	protected override void PerformAdd(GameCard card, int? index, IStackable? stackSrc = null)
	{
		bool wasKnown = card.KnownToEnemy;

		var contexts = IEventContext.Build(Trigger.Rehand)
			.PrimarilyAffecting(card)
			.CausedBy(stackSrc)
			.ForPlayer(Owner)
			.Capture(() => base.PerformAdd(card, index, stackSrc));
		game.StackController.TriggerFor(contexts);

		Networking.ServerNotifier.NotifyRehand(Owner, card, wasKnown);
	}
}