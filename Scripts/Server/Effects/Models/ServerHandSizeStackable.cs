using System.Linq;
using System.Threading.Tasks;
using Godot;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate;
using Kompas.Server.Networking;

namespace KompasServer.Effects
{
	public class ServerHandSizeStackable : HandSizeStackable, IServerStackable
	{
		private bool awaitingChoices;

		private readonly ServerGame serverGame;

		public ServerHandSizeStackable(ServerGame serverGame, IPlayer controller)
			: base(serverGame, controller)
		{
			this.serverGame = serverGame;
			//tell the players this is here now
			ServerNotifier.NotifyHandSizeToStack(controller);
		}
		public async Task StartResolution(IServerResolutionContext context) => await RequestTargets();

		private async Task RequestTargets()
		{
			GD.Print("Trying to request hand size targets");
			awaitingChoices = true;

			var context = new ResolutionContext(new TriggeringEventContext(game: game, stackableCause: this, stackableEvent: this));
			int[] cardIds = game.Cards
				.Where(c => HandSizeCardRestriction.IsValid(c, context))
				.Select(c => c.ID)
				.ToArray();

			int overHandSize = cardIds.Count() - player.HandSizeLimit;
			if (overHandSize <= 0)
			{
				awaitingChoices = false;
				return;
			}

			var listRestriction = IListRestriction.ConstantCount(overHandSize);
			string listRestrictionJson = listRestriction.SerializeToJSON(context);

			int[] choices = null;
			while (!TryAnswer(choices))
			{
				choices = await serverGame.Awaiter.GetHandSizeChoices(player, cardIds, listRestrictionJson);
			}
		}

		public bool TryAnswer(int[] cardIds)
		{
			if (!awaitingChoices) return false;
			if (cardIds == null) return false;

			GameCard[] cards = cardIds
				.Distinct()
				.Select(i => game.LookupCardByID(i))
				.Where(c => c != null)
				.ToArray();

			int count = cards.Count();
			var context = new ResolutionContext(new TriggeringEventContext(game: game, stackableCause: this, stackableEvent: this));
			int correctCount = game.Cards.Count(c => HandSizeCardRestriction.IsValid(c, context)) - player.HandSizeLimit;

			if (count != correctCount || cards.Any(c => !HandSizeCardRestriction.IsValid(c, context))) return false;

			foreach (var card in cards) card.Reshuffle();
			awaitingChoices = false;
			return true;
		}
	}
}